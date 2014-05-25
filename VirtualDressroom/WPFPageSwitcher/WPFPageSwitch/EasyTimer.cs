using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WPFPageSwitch
{
    /// <summary>
    /// Classe permettant d'utiliser de maniere simple les timers
    /// </summary>
    /// <remarks> Thread en mode non STA (utilisation dans les ihm incertaine)</remarks>
   
    public static class EasyTimer
    {
        /// <summary>
        /// Permet de lancer une portion de code a un intervale de temps regulier jusqu'a l'arret
        /// </summary>
        /// <param name="method">code a lancer</param>
        /// <param name="delayInMilliseconds">temporisation</param>
        /// <returns>handle pour stoper notre fonction</returns>
        /// <example> Init<c>var stopHandle = EasyTimer.SetInterval(() =>{--- Your code here --- }, tempo);</c>
        /// Pour stoper <c>stopHandle.Dispose();</c></example>
        public static IDisposable SetInterval(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }

        /// <summary>
        /// Permet de lancer une portion de code au bout d'un certain temps
        /// </summary>
        /// <param name="method">code a lancer</param>
        /// <param name="delayInMilliseconds">temporisation</param>
        /// <returns>handle pour stoper notre fonction</returns>
        /// <example> Init<c>var stopHandle = EasyTimer.SetTimeout(() =>{--- Your code here --- }, tempo);</c>
        /// Pour stoper <c>stopHandle.Dispose();</c></example>
        public static IDisposable SetTimeout(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }


    }
}
