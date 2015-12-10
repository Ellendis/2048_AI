using _2048_Grafos.Classes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace _2048_Grafos
{
    public partial class Jogo : Form
    {
        #region Váriaveis e Propriedades

        //Definir estado atual do jogo
        EstadoJogo jogoAtual = null;
        //Executa um processamento em uma thread separada(para evitar o travamento da interface)
        BackgroundWorker bw = new BackgroundWorker();
        //Desenha os graficos no forms
        Graphics g = null;

        //Armazena o tipo da busca (cega ou heuristica)
        private string Busca { get; set; }

        //Armazena a velocidade da busca
        private int Velocidade { get; set; }

        //Armazena a profundidade da busca
        private int Profundidade { get; set; }
        #endregion

        #region Contrutor

        public Jogo()
        {
            //Inicializa os componentes do Jogo.Design
            InitializeComponent();
            g = this.CreateGraphics(); //Cria e carrega os gráficos da aplicação
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Determina o icone que irá ser utilizado na interface(quando se troca de imagem)
        /// </summary>
        /// <param name="indexIco">número referente ao icone</param>
        /// <returns>Retorna o icone</returns>
        private Image GetPeca(int indexPeca)
        {

            List<Bitmap> listaPecas = new List<Bitmap>() { PecaTabuleiro._0, PecaTabuleiro._2, PecaTabuleiro._4, PecaTabuleiro._8, 
            PecaTabuleiro._16 ,PecaTabuleiro._32,PecaTabuleiro._64,PecaTabuleiro._128,PecaTabuleiro._256,PecaTabuleiro._512,PecaTabuleiro._1024,PecaTabuleiro._2048 };

            return listaPecas[indexPeca];
        }

        /// <summary>
        /// Reseta o jogo e cria um estado inicial aleatorio
        /// </summary>
        public void ResetarJogo()
        {

            
            EstadoJogo novoEstado = new EstadoJogo(4, 4, true); //cria um novo estado para o jogo
            this.jogoAtual = novoEstado; //carrega o novo estado

            //Desenha a imagem especificada, usando seu tamanho físico original, no local especificado.
            g.DrawImage(PecaTabuleiro._Bordas, 0, 0);

            btnAIStart.Text = "Começar Busca";

            RenderStatus(jogoAtual); //Atualizar
        }

        /// <summary>
        /// Sobrescreve a função de pressKey
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //lista de possiveis movimentos.
            List<JogoTabuleiro> movimentoDispon = jogoAtual.getStatusMovimento();
            int count = 0;
            if (movimentoDispon.Count != 0) //se for possivel movimentar...
            {
                bool movimentoTabul = false;

                //Verifica em qual direção o movimento foi feito(de acordo com a entrada do teclado)
                if (keyData == Keys.Left && movimentoDispon.Count(x => x.direct == MoveDir.Esq) > 0) //esquerda
                {
                    movimentoTabul = true;
                    jogoAtual.moverEsq();
                }
                else if (keyData == Keys.Right && movimentoDispon.Count(x => x.direct == MoveDir.Dir) > 0) //direita
                {
                    movimentoTabul = true;
                    jogoAtual.moverDir();
                }
                else if (keyData == Keys.Up && movimentoDispon.Count(x => x.direct == MoveDir.Cima) > 0) //cima
                {
                    movimentoTabul = true;
                    jogoAtual.moverCima();
                }
                else if (keyData == Keys.Down && movimentoDispon.Count(x => x.direct == MoveDir.Baixo) > 0) //baixo
                {
                    movimentoTabul = true;
                    jogoAtual.moverBaixo();
                }

                if (movimentoTabul) //se ocorreu algum movimento 
                {
                    jogoAtual.RespawAleatorio();// gerar a proximo número em local aleatorio.

                    double melhorCont = double.MinValue;
                    List<JogoTabuleiro> melhorMovimento = new List<JogoTabuleiro>();

                    List<JogoTabuleiro> listMovimento = jogoAtual.getStatusMovimento(); //pegar a lista de movimentos

                    foreach (JogoTabuleiro move in listMovimento)
                    {
                        double valorMovimento = 0;
                        switch (this.Busca) //tipo de busca
                        {
                            case "Busca Profundidade":
                                valorMovimento = EstadoJogo.BuscaProfundidade(move.estado, 0, false, ref count);
                                break;
                            case "Busca Profundidade Lmitada":
                                valorMovimento = EstadoJogo.BuscaProfundidade(move.estado, Profundidade, false, ref count);
                                break;
                            case "Busca Heurística":
                                valorMovimento = EstadoJogo.BuscaHeuristica(move.estado, 4, false, ref count);
                                break;
                        }

                        if (valorMovimento > melhorCont)
                        {
                            melhorCont = valorMovimento;
                            melhorMovimento.Clear();
                        }
                        if (valorMovimento == melhorCont)
                        {
                            melhorMovimento.Add(move);
                        }
                    }

                    // Renderizar tabuleiro
                    this.RenderStatus(jogoAtual);
                }
            }
            else
                MessageBox.Show("Fim de jogo! Tente novamente.");
            return true;
        }

        //Renderizar status
        private void RenderStatus(EstadoJogo s)
        {
            try
            {
                int linhas = 4, colunas = 4;
                int imgx = 0, imgy = 0;

                for (int r = 0; r < linhas; r++)
                {
                    imgy = 15 * (r + 1) + (106 * r);
                    for (int c = 0; c < colunas; c++)
                    {
                        int indexIco = s.grid[r, c]; //Pega o código da imagem
                        //Carrega a imagem nova
                        Image tileImage = GetPeca(indexIco);
                        imgx = 15 * (c + 1) + (106 * c);
                        //Desenha a imagem especificada,nas coordenadas especificadas
                        g.DrawImage(tileImage, imgx, imgy, 107, 107);
                    }
                }
                //Força a execução de todas as operações pendentes de gráficos 
                g.Flush();
            }
            catch (Exception ex)
            {
                this.lblGameStatus.Text = "Erro de Renderização: " + ex.Message;
            }
        }

        #endregion

        #region Eventos
        //Botão para resetar o jogo
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            ResetarJogo();
        }

        //Botão de busca
        private void btnAIStart_Click(object sender, EventArgs e)
        {

            if (bw.IsBusy != true)
            {
                btnAIStart.Text = "Pausar Busca";
                //Inicia a execução de um processamento de segundo plano.
                bw.RunWorkerAsync(sender);
            }
            else
            {
                //Cancela o processo em segundo plano.
                bw.CancelAsync();
                btnAIStart.Text = "Retomar Busca";
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Velocidade = int.Parse(cbVeloc.SelectedItem.ToString());
        }

        private void PCGame_Load(object sender, EventArgs e)
        {
            bw.DoWork += new DoWorkEventHandler(bw_DoWork); //tratamento para a chamada do RunWorkerAsync.
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged); //tratamento mudança de processo
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted); //tratamento para a conclusão do processo,cancelamento ou exceção.

            //Obtém ou define um valor indicando se a BackgroundWorker oferece suporte a cancelamento assíncrono.
            bw.WorkerSupportsCancellation = true;

            //Obtém ou define um valor indicando se a BackgroundWorker pode reportar atualizações de andamento.
            bw.WorkerReportsProgress = true;

            Busca = ddlBusca.SelectedText = "Busca Profundidade";
            Profundidade = 1;
            txtExpl.Text = "Busca cega em profundidade"
            + "                   Expande o nó de maior profundidade que esteja na fronteira da árvore de busca.                      Foi usada uma busca por profundidade com prioridade a esquerda,"
            + "na impossibilidade de se "
            + " mover a direita,ele se move a direita,não podendo,vai para cima e depois para baixo,sempre respeitando essa hierarquia";
            cbVeloc.SelectedIndex = 3;
            Velocidade = 1000;
            ResetarJogo();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker process = sender as BackgroundWorker;
            int count = 0;

            if (this.jogoAtual != null)
            {
                EstadoJogo s = jogoAtual; //estado do jogo atual
                while (true) //irá repetir por tempo indeterminado 
                {
                    double melhorCont = double.MinValue;
                    List<JogoTabuleiro> melhorMovimento = new List<JogoTabuleiro>();

                    //movimentos
                    List<JogoTabuleiro> moves = s.getStatusMovimento();

                    foreach (JogoTabuleiro move in moves)
                    {
                        double valorMovimento = 0;
                        switch (this.Busca)
                        {
                            case "Busca Profundidade":
                                valorMovimento = EstadoJogo.BuscaProfundidade(move.estado, 0, false, ref count);
                                count++;
                                break;

                            case "Busca Profundidade Limitada":
                                valorMovimento = EstadoJogo.BuscaProfundidade(move.estado, Profundidade, false, ref count);
                                count++;
                                break;
                            case "Busca Heurística":
                                valorMovimento = EstadoJogo.BuscaHeuristica(move.estado, 4, false, ref count);
                                count++;
                                break;
                        }

                        if (valorMovimento > melhorCont)
                        {
                            melhorCont = valorMovimento;
                            melhorMovimento.Clear();
                        }

                        if (valorMovimento == melhorCont)
                        {
                            melhorMovimento.Add(move);
                        }
                    }

                    if (melhorMovimento.Count == 0) //se não puder mais movimentar,para
                        break;

                    //tempo entre um movimento e outro (busca)
                    int sleepPause = this.Velocidade == 0 ? 1000 : this.Velocidade;

                    //bloquear o processo atual pelo tempo especificado.
                    System.Threading.Thread.Sleep(sleepPause);

                    #region Movimentos Buscas
                    if (melhorMovimento[0].direct == MoveDir.Esq)
                        s.moverEsq();

                    if (melhorMovimento[0].direct == MoveDir.Dir)
                        s.moverDir();

                    if (melhorMovimento[0].direct == MoveDir.Cima)
                        s.moverCima();

                    if (melhorMovimento[0].direct == MoveDir.Baixo)
                        s.moverBaixo();
                    #endregion

                    //Cria um novo número em local aleatorio.
                    s.RespawAleatorio();

                    // atualizar interface
                    process.ReportProgress(0, s);

                    //Obtém um valor que indica se o aplicativo solicitou o cancelamento de um processamento de segundo plano.
                    if ((process.CancellationPending == true))
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
            MessageBox.Show("Vértices visitados: " + count);
        }

        //Evento para tratar a chamada do ReportProgress.
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bool gameover = false;
            EstadoJogo s = e.UserState as EstadoJogo;

            //Criar estado atual
            this.RenderStatus(s);

            // perdeu?
            if (s.getStatusMovimento().Count == 0)
            {
                this.lblGameStatus.Text = "Fail";
                gameover = true;
            }

            // terminou?
            if (s.getMaiorValor() >= 11)
            {
                this.lblGameStatus.Text = "Win";
                gameover = true;
            }

            if (gameover)
            {
                //Cancela o processamento pendente.
                bw.CancelAsync();
                btnAIStart.Text = "Start AI";
                if (this.lblGameStatus.Text == "Win")
                {
                }
                else if (this.lblGameStatus.Text == "Fail")
                {
                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        //Atualizar o jogo
        private void PCGame_Activated(object sender, EventArgs e)
        {
            //Renderizar status do tabuleiro para o estado atual do jogo
            RenderStatus(this.jogoAtual);
        }

        //Evento que captura alterações no dropDown de busca
        private void ddlBusca_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbAlgorithm = sender as ComboBox;
            this.Busca = cbAlgorithm.SelectedItem.ToString();

            label3.Visible = false;
            comboBox1.Enabled = false;
            comboBox1.Visible = false;

            if (this.Busca == "Busca Profundidade")
            {
                txtExpl.Text = "Busca cega em profundidade"
             + "                   Expande o nó de maior profundidade que esteja na fronteira da árvore de busca.                      Foi usada uma busca por profundidade com prioridade a esquerda,na impossibilidade de se "
             + " mover a direita,ele se move a direita,não podendo,vai para cima e depois para baixo,sempre respeitando essa hierarquia";
            }
            else if (this.Busca == "Busca Profundidade Limitada")
            {
                txtExpl.Text = "Busca Profundidade Limitada: Usando uma heurística para terminar o vasculhamento após uma dada profundidade";
                label3.Visible = true;
                comboBox1.Enabled = true;
                comboBox1.Visible = true;
            }
            else
            {
                txtExpl.Text = "Busca Heurística.";

            }
        }

        //Sair do aplicativo
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Sobre
        private void button1_Click(object sender, EventArgs e)
        {
            Sobre sobre = new Sobre();
            sobre.ShowDialog();
        }

        //Profundidade
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Profundidade = int.Parse(comboBox1.SelectedItem.ToString());
        }

        #endregion
    }
}
