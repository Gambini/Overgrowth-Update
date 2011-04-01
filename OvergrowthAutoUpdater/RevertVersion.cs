using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OvergrowthAutoUpdater
{
    public partial class RevertVersion : Form
    {
        ///<summary>Which form opened this one. Not the best name, but we only use it
        ///to access public variables</summary>
        frmMain info;
        ///<summary>When the user clicks Ok, this will be the value that we revert to.
        /// 0 is a value that isn't set.</summary>
        public int retVersion;

        public RevertVersion()
        {
            InitializeComponent();
        }

        public RevertVersion(frmMain sender)
        {
            InitializeComponent();
            info = sender;
            retVersion = 0; //0 is automatically not available
        }


        private void RevertVersion_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Will change the version of your game to the selected value.\n" +
                               "When you click ok here, you can click download and update \n" +
                               "and it will install all of the updates from the version you \n" +
                               "select to the latest version.";

            for (int i = 112; i < info.latestVersion; i++)
            {
                cboxVersions.Items.Add(i);
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            retVersion = (int)cboxVersions.SelectedItem;
        }
    }
}
