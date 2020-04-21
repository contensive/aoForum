
using System;
using Contensive.Addons.Forum.Models.View;
using Contensive.Addons.Forum.Models.Db;
using Contensive.BaseClasses;
using Contensive.Addons.Forum.Controllers;
using System.Collections.Generic;
using Contensive.Models.Db;

namespace Contensive.Addons.Forum {
    namespace Models.View {
        /// <summary>
        /// replies to forum comments
        /// </summary>
        public class ForumCommentReply {
            public int replyId { get; set; }
            public string reply { get; set; }
            public string replyUserName { get; set; }
            public string replyUserEmail { get; set; }
            public string replyImageFilename { get; set; }
        }
        /// <summary>
        /// forum comments
        /// </summary>
        public class ForumComment {
            public int commentId { get; set; }
            public string comment { get; set; }
            public string commentUserName { get; set; }
            public string commentUserEmail { get; set; }
            public string commentByLine { get; set; }
            public string commentImageFilename { get; set; }
            public List<ForumCommentReply> replyList;
        }
        /// <summary>
        /// forum
        /// </summary>
        public class ForumViewModel : DesignBlockViewBaseModel {
            // 
            public int forumId { get; set; }
            public string forumName { get; set; }
            public string headline { get; set; }
            public string description { get; set; }
            public bool recaptcha { get; set; }
            public List<ForumComment> commentList { get; set; }
            public bool userAuthenticated { get; set; }
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
            public static ForumViewModel create(CPBaseClass cp, ForumModel settings, ApplicationController ae) {
                try {
                    // 
                    // -- base fields
                    string avatar = (string.IsNullOrWhiteSpace(ae.user.thumbnailFilename) ? ae.user.imageFilename : ae.user.thumbnailFilename);
                    var result = create<ForumViewModel>(cp, settings);
                    result.userAuthenticated = cp.User.IsAuthenticated;
                    result.addCommentName = ae.user.name;
                    result.addCommentEmail = ae.user.email;
                    result.addCommentImageFilename = DesignBlockController.getAvatarLink(cp, ae, ae.user.thumbnailFilename, ae.user.imageFilename);
                    result.addCommentCopy = "";
                    //
                    //List<ForumCommentModel> commentList = DbBaseModel.createList<ForumCommentModel>(cp, "forumId=" + settings.id + ")","commentid desc,id desc");
                    //foreach ( var comment in commentList) {
                    //    if(comment.commentid.Equals(0)) {
                    //        //
                    //        // -- this is the next newest command (not reply)
                    //    }
                    //}



                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(Contensive.Addon.Forum.Properties.Resources.selectSql.Replace("{forumId}", settings.id.ToString()))) {
                            result.forumId = cs.GetInteger("id");
                            result.forumName = cs.GetText("name");
                            result.headline = cs.GetText("headline");
                            result.description = cs.GetText("description");
                            result.recaptcha = cs.GetBoolean("recaptcha");
                            result.commentList = new List<ForumComment>();
                            //int lastCommentId = 0;
                            ForumComment comment = new ForumComment();
                            ForumCommentReply reply = new ForumCommentReply();
                            do {
                                int commentId = cs.GetInteger("commentId");
                                if (commentId != 0) {
                                    //
                                    // -- this row has a valid comment
                                    if (commentId != comment.commentId) {
                                        //
                                        // -- this row is a new comment
                                        string commentBody = cs.GetText("comment");
                                        string cmName = cs.GetText("cmName");
                                        string cmEmail = cs.GetText("cmEmail");
                                        string commentByLine = (cmName + " " + cmEmail).Trim();
                                        comment = new ForumComment() {
                                            commentId = commentId,
                                            comment = string.IsNullOrWhiteSpace( commentBody ) ? "(empty)" : commentBody ,
                                            commentUserName = cmName,
                                            commentUserEmail = cmEmail,
                                            commentByLine = commentByLine,
                                            commentImageFilename = DesignBlockController.getAvatarLink(cp, ae, cs.GetText("cmthumbfilename"), cs.GetText("cmImageFilename")),
                                            replyList = new List<ForumCommentReply>()
                                        };
                                        result.commentList.Add(comment);
                                    }
                                    int replyId = cs.GetInteger("replyId");
                                    if (replyId != 0) {
                                        //
                                        // -- this row has a valid reply
                                        if (replyId != reply.replyId) {
                                            //
                                            // -- this row has a new reply
                                            string replyImageFilename = cs.GetText("rmImageFilename");
                                            replyImageFilename = (string.IsNullOrWhiteSpace(replyImageFilename)) ? cs.GetText("cmthumbfilename") : replyImageFilename;
                                            replyImageFilename = (!string.IsNullOrWhiteSpace(replyImageFilename)) ? cp.Site.FilePath + replyImageFilename : "";
                                            reply = new ForumCommentReply() {
                                                replyId = cs.GetInteger("replyId"),
                                                reply = cs.GetText("reply"),
                                                replyUserName = cs.GetText("rmName"),
                                                replyUserEmail = cs.GetText("rmEmail"),
                                                replyImageFilename = replyImageFilename
                                            };
                                            comment.replyList.Add(reply);
                                        }
                                    }
                                }
                                cs.GoNext();
                            } while (cs.OK());
                        }
                    }
                    //
                    // -- now reverse the list for display
                    foreach( var comment in result.commentList) {
                        comment.replyList.Reverse();
                    }
                    result.commentList.Reverse();
                    //
                    // -- return the list
                    return result;
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    return null;
                }
            }
        }
    }
}