using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudBackupSystem.util
{
    class ComboBoxItem
    {
        private string _Value = string.Empty;
        private string _Text = string.Empty;

        /// <summary>
        /// value
        /// </summary>
        public string Value
        {
            get { return this._Value; }

            set { this._Value = value; }
        }
        /// <summary>
        /// text
        /// </summary>
        public string Text
        {
            get { return this._Text; }

            set { this._Text = value; }
        }


        public ComboBoxItem(string value, string text)
        {
            this._Value = value;
            this._Text = text;
        }
        public override string ToString()
        {
            return this._Value;
        }

    }
}
