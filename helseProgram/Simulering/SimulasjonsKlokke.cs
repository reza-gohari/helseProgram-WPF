using System;
using System.Diagnostics;

///////////////// simen /////////////////

namespace helseProgram.Simulering
{
    public class SimulasjonsKlokke
    {
        private readonly Stopwatch stoppeklokke = new Stopwatch();

        public TimeSpan SimulasjonsTid => stoppeklokke.Elapsed;

        public void Start()
        {
            if (stoppeklokke.IsRunning)
                return;

            stoppeklokke.Start();
        }

        public void Pause()
        {
            if (!stoppeklokke.IsRunning)
                return;

            stoppeklokke.Stop();
        }

        public void Fortsett()
        {
            if (stoppeklokke.IsRunning)
                return;

            stoppeklokke.Start();
        }

        public void Stopp()
        {
            stoppeklokke.Stop();
        }

        public void Nullstill()
        {
            stoppeklokke.Reset();
        }
    }
}
///////////////// simen /////////////////