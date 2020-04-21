
using System;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.aoForum {
    namespace Models.Db {
        public class ForumCommentModel : DbBaseModel {
            /// <summary>
            /// table definition
            /// </summary>
            public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Forum Comments", "fmForumComments", "default", false);
            //====================================================================================================
            //
            // -- sample model properties. Each property name matches a database field, and the type matches the Contensive use case.
            public int forumid { get; set; }
            public int commentid { get; set; }
            public string imagefilename { get; set; }
            public string comment { get; set; }
            // 
            // ====================================================================================================
        }
    }
}
