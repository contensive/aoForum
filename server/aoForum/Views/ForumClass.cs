
using System;
using Contensive.Addons.Forum.Controllers;
using Contensive.Addons.Forum.Models.Db;
using Contensive.Addons.Forum.Models.View;
using Contensive.BaseClasses;

namespace Contensive.Addons.Forum {
    namespace Views {
        // 
        // ====================================================================================================
        /// <summary>
        ///     ''' Design block with a centered headline, image, paragraph text and a button.
        ///     ''' </summary>
        public class ForumClass : AddonBaseClass {
            //
            private const string guidForumLayout = "{CCD5F2D7-AA59-4397-95FB-C11B1D5CEC39}";
            // 
            // ====================================================================================================
            // 
            public override object Execute(CPBaseClass CP) {
                const string designBlockName = "Forum";
                try {
                    using ( var ae = new ApplicationController(CP)) {
                        // 
                        // -- read instanceId, guid created uniquely for this instance of the addon on a page
                        var result = string.Empty;
                        var settingsGuid = DesignBlockController.getSettingsGuid(CP, designBlockName, ref result);
                        if ((string.IsNullOrEmpty(settingsGuid)))
                            return result;
                        // 
                        // -- locate or create a data record for this guid
                        var settings = ForumModel.createOrAddSettings(CP, settingsGuid);
                        if ((settings == null))
                            throw new ApplicationException("Could not create the design block settings record.");
                        //
                        // -- process buttons
                        ae.processButtonSubmit(CP, settings);
                        // 
                        // -- translate the Db model to a view model and mustache it into the layout
                        var viewModel = ForumViewModel.create(CP, settings, ae);
                        if ((viewModel == null))
                            throw new ApplicationException("Could not create design block view model.");
                        result = Nustache.Core.Render.StringToString(DesignBlockController.getLayoutByGuid(CP, guidForumLayout, "Forum Layout", Contensive.Addon.Forum.Properties.Resources.ForumLayout), viewModel);
                        // 
                        // -- if editing enabled, add the link and wrapperwrapper
                        return CP.Content.GetEditWrapper(result, ForumModel.tableMetadata.contentName, settings.id);
                    }
                } catch (Exception ex) {
                    CP.Site.ErrorReport(ex);
                    return "<!-- " + designBlockName + ", Unexpected Exception -->";
                }
            }
        }
    }
}