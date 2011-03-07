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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            richTextBox1.Text =
                "Originally created by Gambini. Provided open source and free; source at https://github.com/Gambini/Overgrowth-Update/" +
                "\n\nUses DotNetZip to manipulate .zip files. The licence can be found in DotNetZiplicence.txt." +
                "\n\n\nThis program will only work for alpha versions less than a999. If it goes any higher than that, or " +
                "Overgrowth goes in to beta, this updater will not work. You will need to download a newer version.";
        }
    }
}
