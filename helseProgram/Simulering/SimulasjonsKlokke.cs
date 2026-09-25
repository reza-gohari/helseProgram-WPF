using System;

namespace helseProgram.Simulering
{
    public class SimulasjonsKlokke
    {
        private DateTime startTid;
        private TimeSpan oppsamletTid = TimeSpan.Zero;

        private bool kjorer = false;

        public TimeSpan SimulasjonsTid
        {
            get
            {
                if (kjorer)
                {
                    return oppsamletTid + (DateTime.Now - startTid);
                }

                return oppsamletTid;
            }
        }

        public void Start()
        {
            if (kjorer)
                return;

            startTid = DateTime.Now;
            kjorer = true;
        }

        public void Pause()
        {
            if (!kjorer)
                return;

            oppsamletTid += DateTime.Now - startTid;
            kjorer = false;
        }

        public void Fortsett()
        {
            if (kjorer)
                return;

            startTid = DateTime.Now;
            kjorer = true;
        }

        public void Stopp()
        {
            if (kjorer)
            {
                oppsamletTid += DateTime.Now - startTid;
            }

            kjorer = false;
        }

        public void Nullstill()
        {
            oppsamletTid = TimeSpan.Zero;
            kjorer = false;
        }
    }
}