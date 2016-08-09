using System;
using System.Collections.Generic;
using System.IO;

namespace SupervisorioCheetah
{
    class ArquivoTxt
    {
        StreamWriter arquivoEscrita { get; set; }
        StreamReader arquivoLeitura { get; set; }

        void abrirArquivoEscrita(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (Sensores s in Enum.GetValues(typeof(Sensores)))
                {
                    arquivoEscrita = new StreamWriter(File.OpenWrite(path + "\\dados.cvs"));
                    arquivoEscrita.AutoFlush = true;
                }
            }
            catch { }
        }

        void fecharArquivoEscrita()
        {
            try
            {
                arquivoEscrita.Close();
            }
            catch { }
        }

        void salvaArquivo(Queue<double>[] dados)
        {
            if (arquivoEscrita != null) // se existir o arquivo de escrita
            {
                if (dados.Length > 0) // Se o vetor de dados contiver pelo menos um sensor
                {
                    string line = "";
                    bool continua = true;

                    while (continua)
                    {
                        for (int i = 0; i < dados.Length; i++)
                        {
                            line += dados[i].Dequeue().ToString() + (i == (dados.Length - 1) ? "" : ";");

                            continua = dados[i].Count > 0 ? true : false; // Percorre todos os sensores se houver dados em todos os queues
                            // Se não para
                        }

                        arquivoEscrita.WriteLine(line);
                        arquivoEscrita.Flush();
                        line = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Recebe o valor inteiro do caminho incluindo o arquivo</param>
        void abrirArquivoLeitura(string path, Queue<double>[] dados)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    if(File.Exists(path))
                    {
                        double valor = 0;
                        string[] linhas = File.ReadAllLines(path);

                        foreach (string s in linhas)
                        {
                            string[] singleLine = s.Split(';');
                            
                            for (int i = 0; i < singleLine.Length; i++)
                            {
                                dados[i].Enqueue(double.TryParse(singleLine[i], out valor) ? valor : 0);
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}