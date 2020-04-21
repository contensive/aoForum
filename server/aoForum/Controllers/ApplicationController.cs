
using System;
using System.Collections.Generic;
using Contensive.Addons.Forum.Models.Db;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using static Contensive.Addons.Forum.Constants;
using static Newtonsoft.Json.JsonConvert;

namespace Contensive.Addons.Forum {
    namespace Controllers {
        // 
        // ====================================================================================================
        /// <summary>
        ///     ''' 
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        public class ApplicationController : IDisposable {
            // 
            private readonly CPBaseClass cp;
            // 
            // ====================================================================================================
            /// <summary>
            /// Errors accumulated during rendering.
            /// </summary>
            public List<ResponseErrorClass> responseErrorList { get; set; } = new List<ResponseErrorClass>();
            // 
            // ====================================================================================================
            /// <summary>
            /// data accumulated during rendering
            /// </summary>
            public List<ResponseNodeClass> responseNodeList { get; set; } = new List<ResponseNodeClass>();
            // 
            // ====================================================================================================
            /// <summary>
            /// list of name/time used to performance analysis
            /// </summary>
            public List<ResponseProfileClass> responseProfileList { get; set; } = new List<ResponseProfileClass>();
            // 
            // ====================================================================================================
            public PersonModel user {
                get {
                    if (_user == null) { _user = DbBaseModel.create<PersonModel>(cp, cp.User.Id); }
                    return _user;
                }
                set {
                    _user = value;
                }
            }
            private PersonModel _user = null;
            // 
            // ====================================================================================================
            /// <summary>
            /// Constructor, authentication can be disabled
            /// </summary>
            /// <param name="cp"></param>
            /// <param name="requiresAuthentication"></param>
            public ApplicationController(CPBaseClass cp, bool requiresAuthentication) {
                this.cp = cp;
                if ((requiresAuthentication & !cp.User.IsAuthenticated)) {
                    throw new UnauthorizedAccessException();
                }
            }
            // 
            // ====================================================================================================
            /// <summary>
            /// constructor, authentication required
            /// </summary>
            /// <param name="cp"></param>
            public ApplicationController(CPBaseClass cp) {
                this.cp = cp;
            }
            // 
            // ====================================================================================================
            public void processButtonSubmit(CPBaseClass cp, ForumModel settings) {
                try {

                    string button = cp.Doc.GetText("button");
                    if (!string.IsNullOrWhiteSpace(button)) {
                        var buttonParts = button.Split('-');
                        switch (buttonParts[0].ToLowerInvariant()) {
                            case "addcomment": {
                                    //
                                    // -- comment required
                                    string commentBody = cp.Doc.GetText("CommentInput");
                                    if (!string.IsNullOrWhiteSpace(commentBody)) {
                                        //
                                        // -- if not authenticated, add user with name and email
                                        if (!cp.User.IsAuthenticated) {
                                            string commentEmail = cp.Doc.GetText("CommentUserName");
                                            var testUserList = DbBaseModel.createList<PersonModel>(cp, "(email=" + cp.Db.EncodeSQLText(commentEmail) + ")");
                                            if (testUserList.Count.Equals(0)) {
                                                //
                                                // -- the mail is not in use, add the guest as a new user
                                                cp.User.Logout();
                                                user = DbBaseModel.addDefault<PersonModel>(cp);
                                                user.name = cp.Doc.GetText("CommentUserName");
                                                user.email = commentEmail;
                                                user.save(cp);
                                                cp.User.LoginByID(user.id);
                                            }
                                        }
                                        //
                                        // -- save comment
                                        var comment = DbBaseModel.addDefault<ForumCommentModel>(cp);
                                        comment.comment = cp.Doc.GetText("CommentInput");
                                        comment.commentid = 0;
                                        comment.forumid = settings.id;
                                        comment.name = "user " + user.id + ", " + user.name + " comment";
                                        comment.createdBy = cp.User.Id;
                                        comment.dateAdded = DateTime.Now;
                                        comment.modifiedBy = cp.User.Id;
                                        comment.modifiedDate = DateTime.Now;
                                        comment.save(cp);
                                    }
                                    break;
                                }
                            case "addreply": {
                                    //
                                    // -- comment required
                                    int commentId = cp.Utils.EncodeInteger(buttonParts[1]);
                                    string replyBody = cp.Doc.GetText("replyBody" + commentId.ToString());
                                    if (!string.IsNullOrWhiteSpace(replyBody)) {
                                        //
                                        // -- if not authenticated, add user with name and email
                                        if (!cp.User.IsAuthenticated) {
                                            string commentEmail = cp.Doc.GetText("replyUserEmail" + commentId.ToString());
                                            var testUserList = DbBaseModel.createList<PersonModel>(cp, "(email=" + cp.Db.EncodeSQLText(commentEmail) + ")");
                                            if (testUserList.Count.Equals(0)) {
                                                //
                                                // -- the mail is not in use, add the guest as a new user
                                                cp.User.Logout();
                                                user = DbBaseModel.addDefault<PersonModel>(cp);
                                                user.name = cp.Doc.GetText("replyUserEmail" + commentId.ToString());
                                                user.email = commentEmail;
                                                user.save(cp);
                                                cp.User.LoginByID(user.id);
                                            }
                                        }
                                        //
                                        // -- save comment
                                        var reply = DbBaseModel.addDefault<ForumCommentModel>(cp);
                                        reply.comment = replyBody;
                                        reply.commentid = commentId;
                                        reply.forumid = settings.id;
                                        reply.name = "user " + user.id + ", " + user.name + " comment";
                                        reply.createdBy = cp.User.Id;
                                        reply.dateAdded = DateTime.Now;
                                        reply.modifiedBy = cp.User.Id;
                                        reply.modifiedDate = DateTime.Now;
                                        reply.save(cp);
                                    }
                                    break;
                                }
                            default: {
                                    break;
                                }
                        }
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    throw;
                }
            }
            // 
            // ====================================================================================================
            /// <summary>
            /// get the serialized results
            /// </summary>
            /// <returns></returns>
            public string getResponse() {
                try {
                    return SerializeObject(new ResponseClass() {
                        success = responseErrorList.Count.Equals(0),
                        nodeList = responseNodeList,
                        errorList = responseErrorList,
                        profileList = responseProfileList
                    });
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    throw;
                }
            }
            // 
            // ====================================================================================================
            /// <summary>
            /// The user is not authenticated and this activity is not for anonymous access
            /// </summary>
            /// <param name="cp"></param>
            /// <returns></returns>
            public static string getResponseUnauthorized(CPBaseClass cp) {
                cp.Response.SetStatus(HttpErrorEnum.unauthorized + " Unauthorized");
                return string.Empty;
            }
            // 
            // ====================================================================================================
            /// <summary>
            /// The user is authenticated, but their role does not allow this activity
            /// </summary>
            /// <param name="cp"></param>
            /// <returns></returns>
            public static string getResponseForbidden(CPBaseClass cp) {
                cp.Response.SetStatus(HttpErrorEnum.forbidden + " Forbidden");
                return string.Empty;
            }
            // 
            // ====================================================================================================
            //
            public static string getResponseServerError(CPBaseClass cp) {
                cp.Response.SetStatus(HttpErrorEnum.internalServerError + " Internal Server Error");
                return string.Empty;
            }
            // 
            // ==========================================================================================
            // -- Disposable support
            //
            protected bool disposed = false;
            /// <summary>
            /// dispose
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing) {
                if (!this.disposed) {
                    if (disposing) {
                        //
                        // -- dispose non-managed resources
                    }
                }
                this.disposed = true;
            }
            // Do not change or add Overridable to these methods.
            // Put cleanup code in Dispose(ByVal disposing As Boolean).
            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            ~ApplicationController() {
                Dispose(false);
                //base.Finalize();
            }
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// list of events and their stopwatch times
        /// </summary>
        [Serializable()]
        public class ResponseProfileClass {
            public string name;
            public long time;
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// remote method top level data structure
        /// </summary>
        [Serializable()]
        public class ResponseClass {
            public bool success = false;
            public List<ResponseErrorClass> errorList = new List<ResponseErrorClass>();
            public List<ResponseNodeClass> nodeList = new List<ResponseNodeClass>();
            public List<ResponseProfileClass> profileList;
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// data store for jsonPackage
        /// </summary>
        [Serializable()]
        public class ResponseNodeClass {
            public string dataFor = "";
            public object data; // IEnumerable(Of Object)
        }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' error list for jsonPackage
        ///         ''' </summary>
        [Serializable()]
        public class ResponseErrorClass {
            public int number = 0;
            public string description = "";
        }
    }
}
