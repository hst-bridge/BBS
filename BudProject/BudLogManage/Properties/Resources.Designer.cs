﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1022
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BudLogManage.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BudLogManage.Properties.Resources", typeof(Resources).Assembly);
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
        ///   查找类似 すでに存在しています 的本地化字符串。
        /// </summary>
        internal static string existed {
            get {
                return ResourceManager.GetString("existed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 エクスポートファイルに失敗しました 的本地化字符串。
        /// </summary>
        internal static string ExportFailed {
            get {
                return ResourceManager.GetString("ExportFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 ロードに失敗しました 的本地化字符串。
        /// </summary>
        internal static string LoadFailed {
            get {
                return ResourceManager.GetString("LoadFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 いいえ値は導出できない 的本地化字符串。
        /// </summary>
        internal static string NoValueForExport {
            get {
                return ResourceManager.GetString("NoValueForExport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 完全な読み 的本地化字符串。
        /// </summary>
        internal static string ReadComplete {
            get {
                return ResourceManager.GetString("ReadComplete", resourceCulture);
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
        ///   查找类似 エラーログを確認してください 的本地化字符串。
        /// </summary>
        internal static string UnknownError {
            get {
                return ResourceManager.GetString("UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 待ちください 的本地化字符串。
        /// </summary>
        internal static string waiting {
            get {
                return ResourceManager.GetString("waiting", resourceCulture);
            }
        }
    }
}
