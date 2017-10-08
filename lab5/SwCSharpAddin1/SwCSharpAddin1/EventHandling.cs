using System;
using System.Collections;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SwCSharpAddin1
{

    public class DocumentEventHandler
    {
        protected ISldWorks iSwApp;
        protected ModelDoc2 document;
        protected SwAddin userAddin;

        protected Hashtable openModelViews;

        public DocumentEventHandler(ModelDoc2 modDoc, SwAddin addin)
        {
            document = modDoc;
            userAddin = addin;
            iSwApp = (ISldWorks)userAddin.SwApp;
            openModelViews = new Hashtable();
        }

        virtual public bool AttachEventHandlers()
        {
            return true;
        }

        virtual public bool DetachEventHandlers()
        {
            return true;
        }

        public bool ConnectModelViews()
        {
            IModelView mView;
            mView = document.GetFirstModelView();

            while (mView != null)
            {
                if (!openModelViews.Contains(mView))
                {
                    DocView dView = new DocView(userAddin, mView, this);
                    dView.AttachEventHandlers();
                    openModelViews.Add(mView, dView);
                }
                mView = (IModelView)mView.GetNext();
            }
            return true;
        }

        public bool DisconnectModelViews()
        {
            //Close events on all currently open docs
            DocView dView;
            int numKeys;
            numKeys = openModelViews.Count;

            if (numKeys == 0)
            {
                return false;
            }


            object[] keys = new object[numKeys];

            //Remove all ModelView event handlers
            openModelViews.Keys.CopyTo(keys, 0);
            foreach (ModelView key in keys)
            {
                dView = (DocView)openModelViews[key];
                dView.DetachEventHandlers();
                openModelViews.Remove(key);
                dView = null;
            }
            return true;
        }

        public bool DetachModelViewEventHandler(ModelView mView)
        {
            DocView dView;
            if (openModelViews.Contains(mView))
            {
                dView = (DocView)openModelViews[mView];
                openModelViews.Remove(mView);
                mView = null;
                dView = null;
            }
            return true;
        }
    }

    public class PartEventHandler : DocumentEventHandler
    {
        PartDoc doc;

        public PartEventHandler(ModelDoc2 modDoc, SwAddin addin)
            : base(modDoc, addin)
        {
            doc = (PartDoc)document;
        }

        override public bool AttachEventHandlers()
        {
            doc.DestroyNotify += new DPartDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify += new DPartDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            ConnectModelViews();

            return true;
        }

        override public bool DetachEventHandlers()
        {
            doc.DestroyNotify -= new DPartDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify -= new DPartDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            DisconnectModelViews();

            userAddin.DetachModelEventHandler(document);
            return true;
        }

        //Event Handlers
        public int OnDestroy()
        {
            DetachEventHandlers();
            return 0;
        }

        public int OnNewSelection()
        {
            return 0;
        }
    }

    public class AssemblyEventHandler : DocumentEventHandler
    {
        AssemblyDoc doc;
        SwAddin swAddin;

        public AssemblyEventHandler(ModelDoc2 modDoc, SwAddin addin)
            : base(modDoc, addin)
        {
            doc = (AssemblyDoc)document;
            swAddin = addin;
        }

        override public bool AttachEventHandlers()
        {
            doc.DestroyNotify += new DAssemblyDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify += new DAssemblyDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);
            doc.ComponentStateChangeNotify2 += new DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler(ComponentStateChangeNotify2);
            doc.ComponentStateChangeNotify += new DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler(ComponentStateChangeNotify);
            doc.ComponentVisualPropertiesChangeNotify += new DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler(ComponentVisualPropertiesChangeNotify);
            doc.ComponentDisplayStateChangeNotify += new DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler(ComponentDisplayStateChangeNotify);
            ConnectModelViews();

            return true;
        }

        override public bool DetachEventHandlers()
        {
            doc.DestroyNotify -= new DAssemblyDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify -= new DAssemblyDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);
            doc.ComponentStateChangeNotify2 -= new DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler(ComponentStateChangeNotify2);
            doc.ComponentStateChangeNotify -= new DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler(ComponentStateChangeNotify);
            doc.ComponentVisualPropertiesChangeNotify -= new DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler(ComponentVisualPropertiesChangeNotify);
            doc.ComponentDisplayStateChangeNotify -= new DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler(ComponentDisplayStateChangeNotify);
            DisconnectModelViews();

            userAddin.DetachModelEventHandler(document);
            return true;
        }

        //Event Handlers
        public int OnDestroy()
        {
            DetachEventHandlers();
            return 0;
        }

        public int OnNewSelection()
        {
            return 0;
        }

        //attach events to a component if it becomes resolved
        protected int ComponentStateChange(object componentModel, short newCompState)
        {
            ModelDoc2 modDoc = (ModelDoc2)componentModel;
            swComponentSuppressionState_e newState = (swComponentSuppressionState_e)newCompState;


            switch (newState)
            {

                case swComponentSuppressionState_e.swComponentFullyResolved:
                    {
                        if ((modDoc != null) & !this.swAddin.OpenDocs.Contains(modDoc))
                        {
                            this.swAddin.AttachModelDocEventHandler(modDoc);
                        }
                        break;
                    }

                case swComponentSuppressionState_e.swComponentResolved:
                    {
                        if ((modDoc != null) & !this.swAddin.OpenDocs.Contains(modDoc))
                        {
                            this.swAddin.AttachModelDocEventHandler(modDoc);
                        }
                        break;
                    }

            }
            return 0;
        }

        protected int ComponentStateChange(object componentModel)
        {
            ComponentStateChange(componentModel, (short)swComponentSuppressionState_e.swComponentResolved);
            return 0;
        }


        public int ComponentStateChangeNotify2(object componentModel, string CompName, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        int ComponentStateChangeNotify(object componentModel, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        int ComponentDisplayStateChangeNotify(object swObject)
        {
            Component2 component = (Component2)swObject;
            ModelDoc2 modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }

        int ComponentVisualPropertiesChangeNotify(object swObject)
        {
            Component2 component = (Component2)swObject;
            ModelDoc2 modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }




    }

    public class DrawingEventHandler : DocumentEventHandler
    {
        DrawingDoc doc;

        public DrawingEventHandler(ModelDoc2 modDoc, SwAddin addin)
            : base(modDoc, addin)
        {
            doc = (DrawingDoc)document;
        }

        override public bool AttachEventHandlers()
        {
            doc.DestroyNotify += new DDrawingDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify += new DDrawingDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            ConnectModelViews();

            return true;
        }

        override public bool DetachEventHandlers()
        {
            doc.DestroyNotify -= new DDrawingDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify -= new DDrawingDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            DisconnectModelViews();

            userAddin.DetachModelEventHandler(document);
            return true;
        }

        //Event Handlers
        public int OnDestroy()
        {
            DetachEventHandlers();
            return 0;
        }

        public int OnNewSelection()
        {
            return 0;
        }
    }

    public class DocView
    {
        ISldWorks iSwApp;
        SwAddin userAddin;
        ModelView mView;
        DocumentEventHandler parent;

        public DocView(SwAddin addin, IModelView mv, DocumentEventHandler doc)
        {
            userAddin = addin;
            mView = (ModelView)mv;
            iSwApp = (ISldWorks)userAddin.SwApp;
            parent = doc;
        }

        public bool AttachEventHandlers()
        {
            mView.DestroyNotify2 += new DModelViewEvents_DestroyNotify2EventHandler(OnDestroy);
            mView.RepaintNotify += new DModelViewEvents_RepaintNotifyEventHandler(OnRepaint);
            return true;
        }

        public bool DetachEventHandlers()
        {
            mView.DestroyNotify2 -= new DModelViewEvents_DestroyNotify2EventHandler(OnDestroy);
            mView.RepaintNotify -= new DModelViewEvents_RepaintNotifyEventHandler(OnRepaint);
            parent.DetachModelViewEventHandler(mView);
            return true;
        }

        //EventHandlers
        public int OnDestroy(int destroyType)
        {
            return 0;
        }

        public int OnRepaint(int repaintType)
        {
            return 0;
        }
    }

}
