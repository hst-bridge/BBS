﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1022
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BudDBSync.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BudDBSync.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
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
        ///   查找类似 起動に失敗する 的本地化字符串。
        /// </summary>
        internal static string beginFailed {
            get {
                return ResourceManager.GetString("beginFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 待ちください 的本地化字符串。
        /// </summary>
        internal static string DBLinking {
            get {
                return ResourceManager.GetString("DBLinking", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 すでに存在しています 的本地化字符串。
        /// </summary>
        internal static string dsExisted {
            get {
                return ResourceManager.GetString("dsExisted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 データベース接続に失敗しました 的本地化字符串。
        /// </summary>
        internal static string LinkFailed {
            get {
                return ResourceManager.GetString("LinkFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 接続をテストしてください 的本地化字符串。
        /// </summary>
        internal static string LinkMust {
            get {
                return ResourceManager.GetString("LinkMust", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 接続テストが成功しました 的本地化字符串。
        /// </summary>
        internal static string LinkSuccess {
            get {
                return ResourceManager.GetString("LinkSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 記入してください 的本地化字符串。
        /// </summary>
        internal static string NOBlank {
            get {
                return ResourceManager.GetString("NOBlank", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 保存に失敗しました 的本地化字符串。
        /// </summary>
        internal static string SaveFailed {
            get {
                return ResourceManager.GetString("SaveFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 正常に保存 的本地化字符串。
        /// </summary>
        internal static string SaveSuccess {
            get {
                return ResourceManager.GetString("SaveSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 システムエラーが、ログを参照してください 的本地化字符串。
        /// </summary>
        internal static string SystemErrorMsg {
            get {
                return ResourceManager.GetString("SystemErrorMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 システムエラー 的本地化字符串。
        /// </summary>
        internal static string SystemErrorTitle {
            get {
                return ResourceManager.GetString("SystemErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 データベース先が使用できない 的本地化字符串。
        /// </summary>
        internal static string targetUnavailable {
            get {
                return ResourceManager.GetString("targetUnavailable", resourceCulture);
            }
        }
    }
}