
using System;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.Forum {
    namespace Models.Db {
        public class ForumModel : DesignBlockBaseModel {
            /// <summary>
            /// table definition
            /// </summary>
            public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Forums", "fmForums", "default", false);
            //====================================================================================================
            //
            // -- sample model properties. Each property name matches a database field, and the type matches the Contensive use case.
            public string headline { get; set; }
            public string description { get; set; }
            public bool recaptcha { get; set; }
            // 
            // ====================================================================================================
            public static ForumModel createOrAddSettings(CPBaseClass cp, string settingsGuid) {
                ForumModel result = DbBaseModel.create<ForumModel>(cp, settingsGuid);
                if ((result == null)) {
                    // 
                    // -- create default content
                    result = DesignBlockBaseModel.addDefault<ForumModel>(cp);
                    result.name = tableMetadata.contentName + " " + result.id;
                    result.ccguid = settingsGuid;
                    result.recaptcha = false;
                    result.headline = "Lorem Ipsum Dolor Sit Amet";
                    result.description = "<p>Consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>";
                    // 
                    result.save(cp);
                    // 
                    // -- track the last modified date
                    cp.Content.LatestContentModifiedDate.Track(result.modifiedDate);
                }
                return result;
            }
        }
    }
}
