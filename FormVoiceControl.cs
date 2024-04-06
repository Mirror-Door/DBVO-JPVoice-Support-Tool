namespace DBVO_JPVoice_Tool
{
    public partial class FormVoiceControl : Form
    {
        FormMain? f;

        public FormVoiceControl()
        {
            InitializeComponent();
        }

        public FormVoiceControl(FormMain f)
        {
            this.f = f;
            InitializeComponent();
        }

        private void TracktoText(object sender, EventArgs e)
        {
            TrackBarSpeed_Scroll(sender, e);
            TrackBarPitch_Scroll(sender, e);
            TrackBarInto_Scroll(sender, e);
            TrackBarVolume_Scroll(sender, e);
        }

        private void FormVoiceControl_Load(object sender, EventArgs e)
        {
            if (f is not null)
            {
                var Params = ((TextBox)f.Controls["textBoxParam"]).Text.Split(',');
                if (Params.Length == 4)
                {
                    try
                    {
                        trackBarSpeed.Value = Convert.ToInt32(double.Parse(Params[0]) * 100);
                        trackBarPitch.Value = Convert.ToInt32(double.Parse(Params[1]) * 100);
                        trackBarInto.Value = Convert.ToInt32(double.Parse(Params[2]) * 100);
                        trackBarVolume.Value = Convert.ToInt32(double.Parse(Params[3]) * 100);
                    }
                    catch(Exception)
                    {
                        trackBarSpeed.Value = 100;
                        trackBarPitch.Value = 0;
                        trackBarInto.Value = 100;
                        trackBarVolume.Value = 100;
                    }
                }else
                {
                    trackBarSpeed.Value = 100;
                    trackBarPitch.Value = 0;
                    trackBarInto.Value = 100;
                    trackBarVolume.Value = 100;
                }
            }
            TrackBarSpeed_Scroll(trackBarSpeed, e);
            TrackBarPitch_Scroll(trackBarPitch, e);
            TrackBarInto_Scroll(trackBarInto, e);
            TrackBarVolume_Scroll(trackBarVolume, e);
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (f is not null)
            {
                ((TextBox)f.Controls["textBoxParam"]).Text =
                    $"{textBoxSpeed.Text},{textBoxPitch.Text},{textBoxInto.Text},{textBoxVolume.Text}";
            }
            this.Close();
        }

        private void TrackBarSpeed_Scroll(object sender, EventArgs e)
        {
            textBoxSpeed.Text = ((double)((TrackBar)sender).Value / 100).ToString();
        }

        private void TrackBarPitch_Scroll(object sender, EventArgs e)
        {
            textBoxPitch.Text = ((double)((TrackBar)sender).Value / 100).ToString();
        }

        private void TrackBarInto_Scroll(object sender, EventArgs e)
        {
            textBoxInto.Text = ((double)((TrackBar)sender).Value / 100).ToString();
        }

        private void TrackBarVolume_Scroll(object sender, EventArgs e)
        {
            textBoxVolume.Text = ((double)((TrackBar)sender).Value / 100).ToString();
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            trackBarSpeed.Value = 100;
            trackBarPitch.Value = 0;
            trackBarInto.Value = 100;
            trackBarVolume.Value = 100;

            TrackBarSpeed_Scroll(trackBarSpeed, e);
            TrackBarPitch_Scroll(trackBarPitch, e);
            TrackBarInto_Scroll(trackBarInto, e);
            TrackBarVolume_Scroll(trackBarVolume, e);
        }

        private void FormVoiceControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
