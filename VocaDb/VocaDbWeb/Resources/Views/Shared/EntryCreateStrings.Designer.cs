﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ViewRes {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class EntryCreateStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EntryCreateStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VocaDb.Web.Resources.Views.Shared.EntryCreateStrings", typeof(EntryCreateStrings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name in English.
        /// </summary>
        public static string EnglishName {
            get {
                return ResourceManager.GetString("EnglishName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Possible matching entries found in the database. Please verify that these are not duplicates..
        /// </summary>
        public static string FoundPossibleDuplicates {
            get {
                return ResourceManager.GetString("FoundPossibleDuplicates", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name (need at least one).
        /// </summary>
        public static string Name {
            get {
                return ResourceManager.GetString("Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If the original name is non-English (for example Japanese), or if the language of the name is unknown, input that into the Non-English name field. If known, input the romanized name as well. If the official name of the entry is in English, or there is a known English translation, input that into the English name field..
        /// </summary>
        public static string NameHelp {
            get {
                return ResourceManager.GetString("NameHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Need at least one name..
        /// </summary>
        public static string NeedName {
            get {
                return ResourceManager.GetString("NeedName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not enter artist names into the name field. Instead, use the artists list..
        /// </summary>
        public static string NoArtistsToName {
            get {
                return ResourceManager.GetString("NoArtistsToName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Non-English name.
        /// </summary>
        public static string NonEnglishName {
            get {
                return ResourceManager.GetString("NonEnglishName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allowed types: {0}. Maximum size is {1} MB..
        /// </summary>
        public static string PictureInfo {
            get {
                return ResourceManager.GetString("PictureInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Romanized name.
        /// </summary>
        public static string RomajiName {
            get {
                return ResourceManager.GetString("RomajiName", resourceCulture);
            }
        }
    }
}
