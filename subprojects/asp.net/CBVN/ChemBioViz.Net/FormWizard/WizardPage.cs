using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormWizard
{
    public class WizardPage : Form
    {
        private WizardPage _previousPage;
        private WizardPage _nextPage;
        private Button _previousButton;
        private Button _nextButton;

        public WizardPage PreviousPage
        {
            get
            {
                return this._previousPage;
            }
            set
            {
                this._previousPage = value;
                SetButtonLabels();
            }
        }

        public WizardPage NextPage
        {
            get
            {
                return this._nextPage;
            }
            set
            {
                this._nextPage = value;
                SetButtonLabels();
            }
        }

        protected Button PreviousButton
        {
            get
            {
                return _previousButton;
            }
            set
            {
                if (_previousButton != null)
                {
                    _previousButton.Click -= new EventHandler(_previousButton_Click);
                }
                _previousButton = value;
                if (_previousButton != null)
                {
                    _previousButton.Click += new EventHandler(_previousButton_Click);
                    SetButtonLabels();
                }
            }
        }
        
        protected Button NextButton
        {
            get
            {
                return _nextButton;
            }
            set
            {
                if (_nextButton != null)
                {
                    _nextButton.Click -= new EventHandler(_nextButton_Click);
                }
                _nextButton = value;
                if (_nextButton != null)
                {
                    _nextButton.Click += new EventHandler(_nextButton_Click);
                    SetButtonLabels();
                }
            }
        }

        public void GoNext()
        {
            if (this.Visible)
            {
                this.Hide();
                this.NextPage.Show(this.Owner);
            }
        }

        public void GoPrevious()
        {
            if (this.Visible)
            {
                this.Hide();
                this.PreviousPage.Show(this.Owner);
            }
        }

        public delegate void FinishHandler(object sender, EventArgs e);
        public delegate void CancelHandler(object sender, EventArgs e);

        public event FinishHandler Finish;
        public event CancelHandler Cancel;

        protected virtual bool IsValid
        {
            get { return true; }
        }

        protected virtual void OnFinish(EventArgs e)
        {
            if (Finish != null)
            {
                Finish(this, e);
            }
        }

        protected virtual void OnCancel(EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
        }

        private void _previousButton_Click(object sender, EventArgs e)
        {
            if (this.PreviousPage != null)
            {
                GoPrevious();
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                Hide();
                OnCancel(new EventArgs());
            }
        }

        private void _nextButton_Click(object sender, EventArgs e)
        {
            if (this.NextPage != null)
            {
                GoNext();
            }
            else
            {
                if (this.IsValid)
                {
                    this.DialogResult = DialogResult.OK;
                    Hide();
                    OnFinish(new EventArgs());
                }
            }
        }

        private void SetButtonLabels()
        {
            if (this._previousButton != null)
            {
                this._previousButton.Text = (this._previousPage != null) ? "Back" : "Cancel";
            }
            if (this._nextButton != null)
            {
                this._nextButton.Text = (this._nextPage != null) ? "Next" : "Search";
            }
        }
    }
}
