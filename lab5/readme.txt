1. Чтобы появился шаблон проекта плагина (SolidWorks AddIn)в Visual Studio необходимо распаковать содержимое архива swcsharpaddin.zip в папку
\Мои документы\Visual Studio 2013\Templates\ProjectTemplates\Visual C#\sldWorks
и перезапустить Visual Studio
в адресе Visual Studio 2013 должно быть заменено на соответствующую версию

2. Если при компиляции пустого проекта addin возникают трудности, то:
- Добавить ссылку на Microsoft.CShart (Project -> Add References -> Assembles -> Microsoft.CSharp)
- Установить версию платформы .Net на 4.0 (Project -> Properties -> Target FrameWork = .Net 4.0 Client Profile)

3. Если не удается зарегистрировать addin после компиляции, запустите студию от имени администратора  


