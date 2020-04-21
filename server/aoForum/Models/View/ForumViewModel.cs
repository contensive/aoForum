
using System;
using Contensive.Addons.aoForum.Models.View;
using Contensive.Addons.aoForum.Models.Db;
using Contensive.BaseClasses;
using Contensive.Addons.aoForum.Controllers;
using System.Collections.Generic;

namespace Models.View {
    /// <summary>
    /// replies to forum comments
    /// </summary>
    public class ForumCommentReply {
        public string replyImageFilename { get; set; }
        public string reply { get; set; }
    }
    /// <summary>
    /// forum comments
    /// </summary>
    public class ForumComment {
        public string commentImageFilename { get; set; }
        public string comment { get; set; }
        public List<ForumCommentModel> replyList;
    }
    /// <summary>
    /// forum
    /// </summary>
    public class ForumViewModel : DesignBlockViewBaseModel {
        // 
        public string headline { get; set; }
        public string description { get; set; }
        public List<ForumComment> commentList { get; set; }
        public string addCommentImageFilename { get; set; }
        public string addCommentCopy { get; set; }
        public string addCommentEmail { get; set; }
        public string addCommentName { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Populate the view model from the entity model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ForumViewModel create(CPBaseClass cp, ForumModel settings) {
            try {
                // 
                // -- base fields
                var result = create<ForumViewModel>(cp, settings);
                // 
                // -- custom
                //result.imageUrl = string.IsNullOrEmpty(settings.imageFilename) ? "" : (cp.Site.FilePath + settings.imageFilename).Replace(" ", "%20");
                //result.styleAspectRatio = DesignBlockController.getAspectRationStyle(settings.imageAspectRatioId);
                //result.manageAspectRatio = !string.IsNullOrEmpty(result.styleAspectRatio);
                //// 
                //bool isTopElement = string.IsNullOrWhiteSpace(result.imageUrl);
                //result.headline = settings.headline;
                //result.headlineTopPadClass = isTopElement & (!string.IsNullOrEmpty(result.headline)) ? "" : "pt-3";
                //// 
                //isTopElement = isTopElement & string.IsNullOrWhiteSpace(result.headline);
                //result.embed = settings.embed;
                //result.headlineTopPadClass = isTopElement ? "" : "pt-3";
                //// 
                //isTopElement = isTopElement & string.IsNullOrWhiteSpace(result.embed);
                //result.description = settings.description;
                //result.descriptionTopPadClass = isTopElement ? "" : "pt-3";
                //// 
                //isTopElement = isTopElement & string.IsNullOrWhiteSpace(result.description);
                //result.buttonUrl = GenericController.verifyProtocol(settings.buttonUrl);
                //result.buttonText = string.IsNullOrWhiteSpace(settings.buttonText) ? string.Empty : settings.buttonText;
                //result.btnStyleSelector = settings.btnStyleSelector;
                //if ((string.IsNullOrEmpty(result.buttonText) | string.IsNullOrEmpty(result.buttonUrl))) {
                //    result.buttonText = "";
                //    result.buttonUrl = "";
                //}
                //result.buttonTopPadClass = isTopElement ? "" : "pt-3";
                // 
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }
    }
}
