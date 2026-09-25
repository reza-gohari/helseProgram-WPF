using System;

namespace helseProgram.Simulering
{
    public enum SimulasjonsTilstand
    {
        IkkeStartet,
        Kjorer,
        Pauset,
        Avsluttet
    }

    public class SimulasjonsStateMachine
    {
        public SimulasjonsTilstand Tilstand { get; private set; }
            = SimulasjonsTilstand.IkkeStartet;

        public event Action<SimulasjonsTilstand>? TilstandEndret;

        public void Start()
        {
            if (Tilstand != SimulasjonsTilstand.IkkeStartet)
            {
                throw new InvalidOperationException(
                    "Simulasjonen kan bare startes når den ikke allerede er startet.");
            }

            EndreTilstand(SimulasjonsTilstand.Kjorer);
        }

        public void Pause()
        {
            if (Tilstand != SimulasjonsTilstand.Kjorer)
            {
                throw new InvalidOperationException(
                    "Simulasjonen kan bare pauses når den kjører.");
            }

            EndreTilstand(SimulasjonsTilstand.Pauset);
        }

        public void Fortsett()
        {
            if (Tilstand != SimulasjonsTilstand.Pauset)
            {
                throw new InvalidOperationException(
                    "Simulasjonen kan bare fortsette når den er pauset.");
            }

            EndreTilstand(SimulasjonsTilstand.Kjorer);
        }

        public void Stopp()
        {
            if (Tilstand != SimulasjonsTilstand.Kjorer &&
                Tilstand != SimulasjonsTilstand.Pauset)
            {
                throw new InvalidOperationException(
                    "Simulasjonen kan bare stoppes når den kjører eller er pauset.");
            }

            EndreTilstand(SimulasjonsTilstand.Avsluttet);
        }

        private void EndreTilstand(SimulasjonsTilstand nyTilstand)
        {
            Tilstand = nyTilstand;

            TilstandEndret?.Invoke(Tilstand);
        }
    }
}