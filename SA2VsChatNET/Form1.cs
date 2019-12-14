using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SA2VsChatNET
{
    public partial class URLForm : Form
    {
        public URLForm()
        {
            InitializeComponent();
            URLBox.Text = SA2VsChat.YoutubeVideoIDFromSettings;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SA2VsChat.YoutubeVideoIDFromSettings = URLBox.Text;
            Youtube.ReadyToCheck = true;
            this.Close();
        }
    }
}
