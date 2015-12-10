using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace _2048_Grafos.Classes
{
    public class EstadoJogo
    {
        #region Atributos
        static Random random = new Random(); 
        public int[,] grid;
        public int linhas;
        public int colunas;
        #endregion

        #region Construtores

        public EstadoJogo(int linhas, int col)
        {
            this.grid = new int[linhas, col];
            this.linhas = linhas;
            this.colunas = col;
        }

        public EstadoJogo(int l, int c, bool alimentar): this(l, c)
        {
            if (alimentar)
            {
                RespawAleatorio();
                RespawAleatorio();
            }
        }

        public EstadoJogo(EstadoJogo s)
        {
            this.linhas = s.linhas;
            this.colunas = s.colunas;
            this.grid = new int[linhas, colunas];

            for (int r = 0; r < this.linhas; r++)
            {
                for (int c = 0; c < this.colunas; c++)
                {
                    this.grid[r, c] = s.grid[r, c];
                }
            }
        }
        #endregion

        #region Movimentos
        public void moverEsq()
        {
            for (int r = 0; r < this.linhas; r++)
            {
                moveLinhaEsq(r);
            }
        }

        public void moverDir()
        {
            for (int r = 0; r < this.linhas; r++)
            {
                moveLinhaDir(r);
            }
        }

        private void moveLinhaEsq(int r)
        {
            //Pegando os itens
            List<int> items = new List<int>(this.colunas);
            for (int c = 0; c < colunas; c++)
            {
                if (this.grid[r, c] != 0)
                {
                    items.Add(this.grid[r, c]);
                }
            }

            //Consolidando valores duplicados
            for (int i = 0; i < items.Count - 1; i++)
            {
                if (items[i] == items[i + 1])
                {
                    items[i]++;
                    items.RemoveAt(i + 1);
                }
            }

            //Escrevendo os dados na linha
            for (int c = 0; c < colunas; c++)
            {
                this.grid[r, c] = (c < items.Count ? items[c] : 0);
            }
        }

        private void moveLinhaDir(int r)
        {
            //Pegando os itens
            List<int> items = new List<int>(this.colunas);
            for (int c = 0; c < colunas; c++)
            {
                if (this.grid[r, c] != 0)
                {
                    items.Add(this.grid[r, c]);
                }
            }

            //Consolidando valores duplicados
            for (int i = items.Count - 1; i > 0; i--)
            {
                if (items[i] == items[i - 1])
                {
                    items[i]++;
                    items.RemoveAt(i - 1);
                    i--;
                }
            }

            //Escrevendo os dados na linha
            for (int i = 0; i < this.colunas; i++)
                this.grid[r, this.colunas - 1 - i] = (items.Count - 1 - i >= 0 ? items[items.Count - 1 - i] : 0);
        }

        public void moverCima()
        {
            for (int c = 0; c < this.colunas; c++)
            {
                moverColunaCima(c);
            }
        }

        private void moverColunaCima(int c)
        {
            //Pegando os itens
            List<int> items = new List<int>(this.colunas);
            for (int r = 0; r < linhas; r++)
            {
                if (this.grid[r, c] != 0)
                {
                    items.Add(this.grid[r, c]);
                }
            }

            //Removendo valores duplicados
            for (int i = 0; i < items.Count - 1; i++)
            {
                if (items[i] == items[i + 1])
                {
                    items[i]++;
                    items.RemoveAt(i + 1);
                }
            }

            //Escrevendo os dados na linha
            for (int l = 0; l < linhas; l++)
            {
                this.grid[l, c] = (l < items.Count ? items[l] : 0);
            }
        }

        public void moverBaixo()
        {
            for (int c = 0; c < this.colunas; c++)
                moverColunaBaixo(c);
        }

        private void moverColunaBaixo(int c)
        {
            //Pegando os itens
            List<int> items = new List<int>(this.colunas);
            for (int r = 0; r < linhas; r++)
            {
                if (this.grid[r, c] != 0)
                {
                    items.Add(this.grid[r, c]);
                }
            }

            //Consolidando valores duplicados
            for (int i = items.Count - 1; i > 0; i--)
            {
                if (items[i] == items[i - 1])
                {
                    items[i]++;
                    items.RemoveAt(i - 1);
                    i--;
                }
            }

            //Escreve os dados na linha
            for (int i = 0; i < this.linhas; i++)
            {
                this.grid[this.colunas - 1 - i, c] = (items.Count - 1 - i >= 0 ? items[items.Count - 1 - i] : 0);
            }
        }
       
        #endregion

        #region Métodos
        /// <summary>
        /// Avalia os espaços livres na tabela
        /// </summary>
        /// <returns>Retorna os espaços livres na tabela</returns>
        private List<Tuple<int, int>> getEspacosLivres()
        {
            List<Tuple<int, int>> free = new List<Tuple<int, int>>();
            for (int r = 0; r < linhas; r++)
            {
                for (int c = 0; c < colunas; c++)
                {
                    if (this.grid[r, c] == 0)
                    {
                        free.Add(new Tuple<int, int>(r, c));
                    }
                }
            }
            return free;
        }

        /// <summary>
        /// Gerar aleatoriedade
        /// </summary>
        public void RespawAleatorio()
        {
            List<Tuple<int, int>> livre = getEspacosLivres();
            if (livre.Count == 0) //se a tabela estiver cheia,sai da função
                return;

            Tuple<int, int> target = livre[random.Next(0, livre.Count)];
            this.grid[target.Item1, target.Item2] = (random.NextDouble() < .9 ? 1 : 2);
        }

        #region Heuristica para o Nodo(busca profundidade limitada)
        
        public List<JogoTabuleiro> GetSnake(bool horizontal, int[,] board)
        {
            List<JogoTabuleiro> snakeList = new List<JogoTabuleiro>();
            if (horizontal)
            {
                for (int r = 0; r < linhas; r++)
                {
                    for (int c = 0; c < colunas; c++)
                    {
                        if (r % 2 == 0)
                        {
                            snakeList.Add(new JogoTabuleiro(r, c, board[r, c], 4, 0));
                        }
                        else
                        {
                            snakeList.Add(new JogoTabuleiro(r, c, board[r, (colunas - 1) - c], 4, 0));
                        }

                    }
                }
            }
            else
            {
                for (int c = 0; c < colunas; c++)
                {
                    for (int r = 0; r < linhas; r++)
                    {
                        if (c % 2 == 0)
                        {
                            snakeList.Add(new JogoTabuleiro(r, c, board[r, c], 4, 0));
                        }
                        else
                        {
                            snakeList.Add(new JogoTabuleiro((linhas - 1) - r, c, board[r, c], 4, 0));
                        }
                    }
                }
            }
            return snakeList;
        }
        
        public double SnakeRating()
        {
            List<JogoTabuleiro>[] snakes = new List<JogoTabuleiro>[2];
            snakes[0] = GetSnake(true, this.grid);
            snakes[1] = GetSnake(false, this.grid);

            double score = 0;
            double bestScore = 0;
            double weight = 1;
            for (int x = 0; x < snakes.Length; x++)
            {
                score = 0;
                weight = 1;
                for (int i = 0; i < snakes[x].Count; i++)
                {
                    score += Math.Pow(2, snakes[x][i].valor) * weight;
                    weight *= 0.25;
                }
                if (score > bestScore)
                    bestScore = score;
            }
            return bestScore;
        }
   
        public double rate()
        {
            return SnakeRating();
        }
        
        #endregion

        #region Busca Heurística (MiniMax)
        
        //Calcula o provável filho
        private static double provavelFilho(int possibleChildren, int childIndex)
        {
            double probability = 0;
            double chance = 0;
            double twoChild = possibleChildren / 2;
            chance = (childIndex % 2 == 0) ? 0.9 : 0.1;
            probability = (double)(1 / twoChild) * chance;
            return probability;
        }

        public static double BuscaHeuristica(EstadoJogo raiz, int profund , bool player, ref int count)
        {
            double melhorValor = 0;
            double val = 0;
            if (profund == 0)
            {
                count++;
                return raiz.rate();
            }
            if (player)
            {
                melhorValor = double.MinValue;
                List<JogoTabuleiro> movimento = raiz.getStatusMovimento();

                if (movimento.Count == 0)
                    return double.MinValue;

                foreach (JogoTabuleiro st in movimento)
                {
                    count++;
                    val = BuscaHeuristica(st.estado, profund - 1, false, ref count);
                    melhorValor = Math.Max(val, melhorValor);
                }
                return melhorValor;
            }
            else
            {
                melhorValor = 0;
                List<EstadoJogo> moves = raiz.getTotalAleatorio();
                int i = 0;
                foreach (EstadoJogo st in moves)
                {
                    count++;
                    melhorValor += (provavelFilho(moves.Count, i) * BuscaHeuristica(st, profund - 1, true, ref count));
                    i++;
                }

                return melhorValor;
            }
        }

        #endregion

        /// <summary>
        /// Achar o maior valor da tabela
        /// </summary>
        /// <returns>Retorna o maior valor da tabela</returns>
        public double getMaiorValor()
        {
            int max = 0;
            for (var x = 0; x < linhas; x++)
            {
                for (var y = 0; y < colunas; y++)
                {
                    if (this.grid[x, y] != 0)
                    {
                        int value = this.grid[x, y];
                        if (value > max)
                        {
                            max = value;
                        }
                    }
                }
            }
            double result = max;
            return result;
        }

        public static double BuscaProfundidade(EstadoJogo raiz, int profund, bool player,ref int count)
        {
            double melhorValor = 0;
            double val = 0;

            if (profund == 0)
            {
                return raiz.rate();
            }
            if (player)
            {
                melhorValor = double.MinValue;
                List<JogoTabuleiro> movimentos = raiz.getStatusMovimento();

                //se não puder se mover o jogo acabou
                if (movimentos.Count == 0)
                {
                    return double.MinValue;
                }

                foreach (JogoTabuleiro st in movimentos)
                {
                    count++;
                    val = BuscaProfundidade(st.estado, profund - 1, false,ref count);
                    melhorValor = Math.Max(val, melhorValor);
                }
                return melhorValor;
            }
            else
            {
                melhorValor = double.MaxValue;
                List<EstadoJogo> movimentos = raiz.getTotalAleatorio();

                foreach (EstadoJogo es in movimentos)
                {
                    count++;
                    val = BuscaProfundidade(es, profund - 1, true,ref count);
                    melhorValor = Math.Min(val, melhorValor);
                }

                return melhorValor;
            }
        }

        /// <summary>
        /// Pegar a direção dos melhores movimentos
        /// </summary>
        /// <returns>Retorna uma lista de direções</returns>
        public List<JogoTabuleiro> getStatusMovimento()
        {
            List<JogoTabuleiro> geralMovimento = new List<JogoTabuleiro>();

            EstadoJogo next;

            next = new EstadoJogo(this);
            next.moverEsq();
            if (!this.verificarEstado(next))
                geralMovimento.Add(new JogoTabuleiro(next, MoveDir.Esq));

            next = new EstadoJogo(this);
            next.moverDir();
            if (!this.verificarEstado(next))
                geralMovimento.Add(new JogoTabuleiro(next, MoveDir.Dir));

            next = new EstadoJogo(this);
            next.moverCima();
            if (!this.verificarEstado(next))
                geralMovimento.Add(new JogoTabuleiro(next, MoveDir.Cima));

            next = new EstadoJogo(this);
            next.moverBaixo();
            if (!this.verificarEstado(next))
                geralMovimento.Add(new JogoTabuleiro(next, MoveDir.Baixo));

            return geralMovimento;
        }

        public List<EstadoJogo> getTotalAleatorio()
        {
            List<EstadoJogo> res = new List<EstadoJogo>();
            List<Tuple<int, int>> livre = this.getEspacosLivres();

            foreach (Tuple<int, int> x in livre)
            {
                EstadoJogo next;

                next = new EstadoJogo(this);
                next.grid[x.Item1, x.Item2] = 1;
                res.Add(next);

                next = new EstadoJogo(this);
                next.grid[x.Item1, x.Item2] = 2;
                res.Add(next);
            }

            return res;
        }

        /// <summary>
        /// Verificar estado do jogo
        /// </summary>
        /// <param name="alt">Estado atual do jogo</param>
        /// <returns></returns>
        public bool verificarEstado(EstadoJogo alt)
        {
            if (this.linhas != alt.linhas)
            {
                return false;
            }
            if (this.colunas != alt.colunas)
            {
                return false;
            }

            for (int r = 0; r < linhas; r++)
            {
                for (int c = 0; c < colunas; c++)
                {
                    if (this.grid[r, c] != alt.grid[r, c])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Sobrescreve a função nativa ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int l = 0; l < this.linhas; l++)
            {
                for (int c = 0; c < this.colunas; c++)
                {
                    string number = "";

                    if (this.grid[l, c] != 0)
                    {
                        number = ((int)Math.Pow(2, this.grid[l, c])).ToString();
                    }
                    sb.Append(number.PadLeft(5, ' '));

                }
                sb.AppendLine();

            }
            sb.AppendLine(" ---- ---- ---- ----");

            return sb.ToString();
        }
        #endregion
    }
}
