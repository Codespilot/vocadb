//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option or rebuild the Visual Studio project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "10.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CommonMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommonMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.CommonMessages", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This entry is a draft. You can help by editing the entry and adding the missing information..
        /// </summary>
        internal static string Draft {
            get {
                return ResourceManager.GetString("Draft", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to These entries are marked as drafts which means they are missing some information. .
        /// </summary>
        internal static string Drafts {
            get {
                return ResourceManager.GetString("Drafts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This entry has been deleted. It is still temporarily accessible, but won&apos;t show up in any of the listings..
        /// </summary>
        internal static string EntryDeleted {
            get {
                return ResourceManager.GetString("EntryDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This entry is locked, meaning that only trusted users are allowed to edit it..
        /// </summary>
        internal static string Locked {
            get {
                return ResourceManager.GetString("Locked", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If the entry has a non-English (for example Japanese) name, input that into the Non-English name field. If known, input the romanized name as well. If the official name of the entry is in English, or the there is a known English translation, input that into the English name field..
        /// </summary>
        internal static string NameHelp {
            get {
                return ResourceManager.GetString("NameHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose the language for this name. &quot;Original&quot; is the name in original language that isn&apos;t English, for example Japanese. If the original language is English, do not input a name in the &quot;Original&quot; language..
        /// </summary>
        internal static string NameLanguageHelp {
            get {
                return ResourceManager.GetString("NameLanguageHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not enter artist names into the name field. Instead, use the artists list..
        /// </summary>
        internal static string NoArtistsToName {
            get {
                return ResourceManager.GetString("NoArtistsToName", resourceCulture);
            }
        }
    }
}
