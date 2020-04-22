
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
            public string replyByLine { get; set; }
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
                int hint = 10;
                try {
                    // 
                    // -- base fields
                    string avatar = (string.IsNullOrWhiteSpace(ae.user.thumbnailFilename) ? ae.user.imageFilename : ae.user.thumbnailFilename);
                    var result = create<ForumViewModel>(cp, settings);
                    result.forumId = settings.id;
                    result.forumName = settings.name;
                    result.headline = settings.headline;
                    result.description = settings.description;
                    result.recaptcha = settings.recaptcha;
                    result.userAuthenticated = cp.User.IsAuthenticated;
                    result.addCommentName = ae.user.name;
                    result.addCommentEmail = ae.user.email;
                    result.addCommentImageFilename = DesignBlockController.getAvatarLink(cp, ae, ae.user.thumbnailFilename, ae.user.imageFilename);
                    result.addCommentCopy = "";
                    result.commentList = new List<ForumComment>();
                    using (var cs = cp.CSNew()) {
                        hint = 20;
                        if (cs.OpenSQL(Contensive.Addon.Forum.Properties.Resources.selectSql.Replace("{forumId}", settings.id.ToString()))) {
                            //int lastCommentId = 0;
                            ForumComment comment = new ForumComment();
                            ForumCommentReply reply = new ForumCommentReply();
                            do {
                                hint = 30;
                                int commentId = cs.GetInteger("commentId");
                                if (commentId != 0) {
                                    hint = 40;
                                    //
                                    // -- this row has a valid comment
                                    if (commentId != comment.commentId) {
                                        hint = 50;
                                        //
                                        // -- this row is a new comment
                                        string commentBody = cs.GetText("comment");
                                        string cmName = cs.GetText("cmName");
                                        string cmEmail = cs.GetText("cmEmail");
                                        DateTime cDateAdded = cs.GetDate("cDateAdded");
                                        DateTime rightNow = DateTime.Now;
                                        string cDateCaption = (rightNow.AddDays(7).CompareTo(cDateAdded) > 0) ? cDateAdded.ToString("dddd, d MMMM") : cDateAdded.ToString("d MMMM yyyy");
                                        string commentByLine = cmName;
                                        commentByLine += ((!string.IsNullOrWhiteSpace(commentByLine) && !string.IsNullOrWhiteSpace(cDateCaption)) ? " &bull; " : "") + cDateCaption;
                                        commentByLine += ((!string.IsNullOrWhiteSpace(commentByLine)) ? " &bull; " : "");
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
                                    hint = 60;
                                    int replyId = cs.GetInteger("replyId");
                                    if (replyId != 0) {
                                        hint = 70;
                                        //
                                        // -- this row has a valid reply
                                        if (replyId != reply.replyId) {
                                            //
                                            // -- this row has a new reply
                                            DateTime rDateAdded = cs.GetDate("rDateAdded");
                                            DateTime rightNow = DateTime.Now;
                                            string rDateCaption = (rightNow.AddDays(7).CompareTo(rDateAdded) > 0) ? rDateAdded.ToString("dddd, d MMMM") : rDateAdded.ToString("d MMMM yyyy");
                                            string replyByLine = cs.GetText("rmName");
                                            replyByLine += ((!string.IsNullOrWhiteSpace(replyByLine) && !string.IsNullOrWhiteSpace(rDateCaption)) ? " &bull; " : "") + rDateCaption;
                                            reply = new ForumCommentReply() {
                                                replyId = cs.GetInteger("replyId"),
                                                reply = cs.GetText("reply"),
                                                replyUserName = cs.GetText("rmName"),
                                                replyUserEmail = cs.GetText("rmEmail"),
                                                replyByLine = replyByLine,
                                                replyImageFilename = DesignBlockController.getAvatarLink(cp, ae, cs.GetText("cmthumbfilename"), cs.GetText("rmImageFilename"))
                                            };
                                            comment.replyList.Add(reply);
                                        }
                                    }
                                }
                                hint = 80;
                                cs.GoNext();
                            } while (cs.OK());
                        }
                    }
                    //
                    // -- now reverse the list for display
                    hint = 90;
                    foreach ( var comment in result.commentList) {
                        comment.replyList.Reverse();
                    }
                    result.commentList.Reverse();
                    //
                    // -- return the list
                    return result;
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "hint-" + hint.ToString());
                    return null;
                }
            }
        }
    }
}