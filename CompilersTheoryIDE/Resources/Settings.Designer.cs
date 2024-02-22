namespace CompilersTheoryIDE.Resources;

[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.3.0.0")]
internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
    public static Settings Default {
        get {
            return defaultInstance;
        }
    }
        
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("en-US")]
    public global::System.Globalization.CultureInfo DefaultLanguage {
        get {
            return ((global::System.Globalization.CultureInfo)(this["DefaultLanguage"]));
        }
        set {
            this["DefaultLanguage"] = value;
        }
    }
}