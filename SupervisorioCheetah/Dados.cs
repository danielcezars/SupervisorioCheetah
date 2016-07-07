using System;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Classe com todos os dados do carro
    /// Verifica se cada um dos dados já foi adicionado
    /// </summary>
    public class Dados
    {
        private DateTime tempo;
        private double acelX;
        private double acelY;
        private double acelZ;
        private double yaw;
        private double pitch;
        private double roll;
        private double latitude;
        private double longitude;
        private double posicaoX;
        private double posicaoY;
        private int anguloVolante;
        private int velocidade1;
        private int velocidade2;
        private int velocidade3;
        private int velocidade4;
        private int velocidadeCarro;
        private int velocidadeGPS;
        private int freioDianteiro;
        private int freioTraseiro;
        private int rotacaoMotor;



        public bool _tempo { get; }
        public bool _acelX { get; }
        public bool _acelY { get; }
        public bool _acelZ { get; }
        public bool _yaw { get; }
        public bool _pitch { get; }
        public bool _roll { get; }
        public bool _Latitude { get; }
        public bool _Longitude { get; }
        public bool _posicaoX { get; }
        public bool _posicaoY { get; }
        public bool _anguloVolante { get; }
        public bool _velocidade1 { get; }
        public bool _velocidade2 { get; }
        public bool _velocidade3 { get; }
        public bool _velocidade4 { get; }
        public bool _velocidadeCarro { get; }
        public bool _velocidadeGPS { get; }
        public bool _freioDianteiro { get; }
        public bool _freioTraseiro { get; }
        public bool _rotacaoMotor { get; }

        public DateTime Tempo
        {
            get
            {
                return tempo;
            }

            set
            {
                _tempo = true;
                tempo = value;
            }
        }

        public double AcelX
        {
            get
            {
                return acelX;
            }

            set
            {
                _acelX = true;
                acelX = value;
            }
        }

        public double AcelY
        {
            get
            {
                return acelY;
            }

            set
            {
                _acelY = true;
                acelY = value;
            }
        }

        public double AcelZ
        {
            get
            {
                return acelZ;
            }

            set
            {
                _acelZ = true;
                acelZ = value;
            }
        }

        public double Yaw
        {
            get
            {
                return yaw;
            }

            set
            {
                _yaw = true;
                yaw = value;
            }
        }

        public double Pitch
        {
            get
            {
                return pitch;
            }

            set
            {
                _pitch = true;
                pitch = value;
            }
        }

        public double Roll
        {
            get
            {
                return roll;
            }

            set
            {
                _roll = true;
                roll = value;
            }
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }

            set
            {
                _Latitude = true;
                latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }

            set
            {
                _Longitude = true;
                longitude = value;
            }
        }

        public double PosicaoX
        {
            get
            {
                return posicaoX;
            }

            set
            {
                _posicaoX = true;
                posicaoX = value;
            }
        }

        public double PosicaoY
        {
            get
            {
                return posicaoY;
            }

            set
            {
                _posicaoY = true;
                posicaoY = value;
            }
        }

        public int AnguloVolante
        {
            get
            {
                return anguloVolante;
            }

            set
            {
                _anguloVolante = true;
                anguloVolante = value;
            }
        }

        public int Velocidade1
        {
            get
            {
                return velocidade1;
            }

            set
            {
                _velocidade1 = true;
                velocidade1 = value;
            }
        }

        public int Velocidade2
        {
            get
            {
                return velocidade2;
            }

            set
            {
                _velocidade2 = true;
                velocidade2 = value;
            }
        }

        public int Velocidade3
        {
            get
            {
                return velocidade3;
            }

            set
            {
                _velocidade3 = true;
                velocidade3 = value;
            }
        }

        public int Velocidade4
        {
            get
            {
                return velocidade4;
            }

            set
            {
                _velocidade4 = true;
                velocidade4 = value;
            }
        }

        public int VelocidadeCarro
        {
            get
            {
                return velocidadeCarro;
            }

            set
            {
                _velocidadeCarro = true;
                velocidadeCarro = value;
            }
        }

        public int VelocidadeGPS
        {
            get
            {
                return velocidadeGPS;
            }

            set
            {
                _velocidadeGPS = true;
                velocidadeGPS = value;
            }
        }

        public int FreioDianteiro
        {
            get
            {
                return freioDianteiro;
            }

            set
            {
                _freioDianteiro = true;
                freioDianteiro = value;
            }
        }

        public int FreioTraseiro
        {
            get
            {
                return freioTraseiro;
            }

            set
            {
                _freioTraseiro = true;
                freioTraseiro = value;
            }
        }

        public int RotacaoMotor
        {
            get
            {
                return rotacaoMotor;
            }

            set
            {
                _rotacaoMotor = true;
                rotacaoMotor = value;
            }
        }

        public Dados()
        {
            Tempo = new DateTime();
            AcelX = 0;
            AcelY = 0;
            AcelZ = 0;
            Yaw = 0;
            Pitch = 0;
            Roll = 0;
            Latitude = 0;
            Longitude = 0;
            PosicaoX = 0;
            PosicaoY = 0;
            AnguloVolante = 0;
            Velocidade1 = 0;
            Velocidade2 = 0;
            Velocidade3 = 0;
            Velocidade4 = 0;
            VelocidadeCarro = 0;
            VelocidadeGPS = 0;
            FreioDianteiro = 0;
            FreioTraseiro = 0;
            RotacaoMotor = 0;



            _tempo = false;
            _acelX = false;
            _acelY = false;
            _acelZ = false;
            _yaw = false;
            _pitch = false;
            _roll = false;
            _Latitude = false;
            _Longitude = false;
            _posicaoX = false;
            _posicaoY = false;
            _anguloVolante = false;
            _velocidade1 = false;
            _velocidade2 = false;
            _velocidade3 = false;
            _velocidade4 = false;
            _velocidadeCarro = false;
            _velocidadeGPS = false;
            _freioDianteiro = false;
            _freioTraseiro = false;
            _rotacaoMotor = false;
        }
    }
}