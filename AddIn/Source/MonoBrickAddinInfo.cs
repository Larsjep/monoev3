using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"MonoBrickAddin",
	Namespace = "MonoBrickAddin",
	Version = "0.3",
	Url = "http://github.com/Larsjep/monoev3"
)]

[assembly:AddinName ("MonoBrickAddin")]
[assembly:AddinCategory ("Mindstorms")]
[assembly:AddinDescription ("MonoBrick solution and debugging")]
[assembly:AddinAuthor ("Bernhard Straub")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Debugger", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Debugger.Soft", MonoDevelop.BuildInfo.Version)]