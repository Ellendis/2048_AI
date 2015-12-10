using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048_Grafos.Classes
{
    public enum MoveDir { Esq, Dir, Cima, Baixo };

    public class JogoTabuleiro 
    {
        #region Atributos
        public int linha;
        public int coluna;
        public int valor;
        public int tabela;
        public int profund;
        public MoveDir direct;
        public EstadoJogo estado;


        #endregion
        //Construtor
        public JogoTabuleiro(int l, int c, int v, int grid, int prof)
        {
            this.linha = l;
            this.coluna = c;
            this.valor = v;
            this.tabela = grid;
            this.profund = prof;
        }

        //Construtor
        public JogoTabuleiro(EstadoJogo estado, MoveDir direct)
        {
            this.direct = direct;
            this.estado = estado;
        }

        /// <summary>
        /// Sobrepoe a função de conversão ToString
        /// </summary>
        /// <returns>Retorna a string personalizada</returns>
        public override string ToString()
        {
            string valstr = valor >= 1 ? Math.Pow(2, valor).ToString() : "0";
            return valstr + "(" + linha + "," + coluna + ") ";
        }
    }
}
