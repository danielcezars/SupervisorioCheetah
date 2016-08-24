using System;
using System.Collections.Generic;
using System.IO;

namespace SupervisorioCheetah
{
    public static class ArquivoTxt
    {
        public static void salvaArquivo(string path, Queue<double>[] dados)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (dados.Length > 0) // Se o vetor de dados contiver pelo menos um sensor
                {
                    string[] tudo = new string[dados[0].Count];
                    string line = "";
                    bool continua = true;
                    int n = 0;
                    while (continua)
                    {
                        for (int i = 0; i < dados.Length; i++) // percorre o vetor
                        {
                            line += dados[i].Dequeue().ToString() + (i == (dados.Length - 1) ? "" : ";");

                            continua = dados[i].Count > 0 && continua; // Percorre todos os sensores se houver dados em todos os queues
                            // Se não para
                        }
                        tudo[n] = line;
                        line = string.Empty;
                        n++;
                    }
                    File.WriteAllLines(path + "\\dados.cvs", tudo);
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Recebe o valor inteiro do caminho incluindo o arquivo</param>
        public static void lerArquivo(string path, Queue<double>[] dados)
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