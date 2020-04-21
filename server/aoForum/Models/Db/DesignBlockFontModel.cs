
using Contensive.Models.Db;

namespace Contensive.Addons.aoForum {
    namespace Models.Db {
        // 
        /// <summary>
        ///     ''' This model provides the common fields for all Design Blocks.
        ///     ''' </summary>
        public class DesignBlockFontModel : DbBaseModel {
            // 
            // ====================================================================================================
            /// <summary>
            /// metadata
            /// </summary>
            public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("Design Block Fonts", "dbfonts", "default", false);
            // 
            // ====================================================================================================
            // -- instance properties
        }
    }
}