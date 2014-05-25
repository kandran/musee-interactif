using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace WPFPageSwitch
{


    /// <summary>
    /// 
    /// </summary>
    public class GestureButton : Button
    {
        /// <summary>
        /// Occurs when [action entry].
        /// </summary>
        public event EventHandler<ActionEventArgs> ActionEntry;

        /// <summary>
        /// Occurs when [action exit].
        /// </summary>
        public event EventHandler<ActionEventArgs> ActionExit;

        /// <summary>
        /// Occurs when [action completed].
        /// </summary>
        public event EventHandler<ActionEventArgs> ActionCompleted;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ActionStatus Status { get; set; }

        /// <summary>
        /// Double Animation
        /// </summary>
        private DoubleAnimation buttonEffects;

        /// <summary>
        /// Gets or sets the hand cursor.
        /// </summary>
        /// <value>
        /// The hand cursor.
        /// </value>
        public HandCursor HandCursor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureButton" /> class.
        /// </summary>
        public GestureButton()
        {
            this.ActionCompleted += new EventHandler<ActionEventArgs>(GestureButton_ActionCompleted);
            this.ActionEntry += new EventHandler<ActionEventArgs>(GestureButton_ActionEntry);
            this.ActionExit += new EventHandler<ActionEventArgs>(GestureButton_ActionExit);
        }

        /// <summary>
        /// Handles the ActionExit event of the GestureButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        void GestureButton_ActionExit(object sender, ActionEventArgs e)
        {
            if (this.Status == ActionStatus.Enter)
            {
                this.Status = ActionStatus.Exit;
                buttonEffects.Completed -= new EventHandler(effectsCompleted);
                buttonEffects = new DoubleAnimation(20, 12, new Duration(new TimeSpan(0, 0, 2)));
                this.BeginAnimation(TextBlock.FontSizeProperty, buttonEffects);

            }
        }

        /// <summary>
        /// Handles the ActionEntry event of the GestureButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        void GestureButton_ActionEntry(object sender, ActionEventArgs e)
        {
            if (this.Status == ActionStatus.NotStarted || this.Status == ActionStatus.Exit || this.Status == ActionStatus.Completed)
            {
                this.Status = ActionStatus.Enter;
                buttonEffects = new DoubleAnimation(12, 20, new Duration(new TimeSpan(0, 0, 2)));
                buttonEffects.Completed += new EventHandler(effectsCompleted);
                this.BeginAnimation(TextBlock.FontSizeProperty, buttonEffects);
            }
        }

        /// <summary>
        /// Handles the ActionCompleted event of the GestureButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        void GestureButton_ActionCompleted(object sender, ActionEventArgs e)
        {
            this.Status = ActionStatus.Completed;
            this.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// Validates the poisition.
        /// </summary>
        /// <param name="handCursor">The hand cursor.</param>
        /// <returns></returns>
        public bool ValidatePoisition(HandCursor handCursor)
        {
            try
            {
                CursorPoint cursorPoint = handCursor.GetCursorPoint();
                ButtonPosition buttonPostion = this.GetPosition();
                if ((cursorPoint.X < buttonPostion.Left || cursorPoint.X > buttonPostion.Right) || cursorPoint.Y < buttonPostion.Top || cursorPoint.Y > buttonPostion.Bottom)
                {
                    if (this.ActionExit != null)
                    {
                        this.ActionExit(this, new ActionEventArgs(ActionStatus.Exit));
                    }
                    return false;
                }
      
                if (this.ActionEntry != null)
                {
                    this.ActionEntry(this, new ActionEventArgs(ActionStatus.Completed));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Effectses the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void effectsCompleted(object sender, EventArgs e)
        {
            if (this.ActionCompleted != null)
            {
                this.ActionCompleted(this, new ActionEventArgs(ActionStatus.Completed));
            }
            buttonEffects.Completed -= new EventHandler(effectsCompleted);
            this.BeginAnimation(TextBlock.FontSizeProperty, null);
        }


    }
}
