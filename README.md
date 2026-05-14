# The Wood's Tale

**The Wood's Tale** é um jogo de plataforma 2D de suspense e aventura, no qual o jogador precisa escapar de uma floresta misteriosa habitada por monstros e demônios.

## Links

- **GDD (Milanote):** https://app.milanote.com/1WhIkj1staAB2R?p=RlW3wDwJmK8

## História

A história começa quando um grupo de amigos vai acampar e alguém conta a lenda conhecida como *The Wood's Tale*, uma história assustadora sobre uma floresta amaldiçoada. Durante a noite, o personagem principal é desafiado pelos amigos a entrar na floresta, que todos acreditavam ser apenas parte da lenda.

Ao avançar, ele descobre que os rumores eram reais: a floresta possui criaturas perigosas, caminhos instáveis, puzzles e áreas sombrias que dificultam sua fuga.

## Personagem

O jogador controla um jovem que entrou na floresta por desafio dos amigos. No começo, ele não acredita totalmente na lenda, mas rapidamente percebe que está preso em um ambiente hostil e precisa encontrar uma forma de escapar — atravessando plataformas, resolvendo puzzles, evitando monstros e alcançando pontos de saída como cavernas, portas ou passagens escondidas que levam para novos níveis.

## Cenário

O jogo se passa em uma floresta sombria, com diferentes áreas e níveis de profundidade. O mapa possui:

- Plataformas em diferentes altitudes
- Plataformas móveis
- Áreas fechadas por obstáculos
- Cavernas ou portas que levam para novas fases
- Elementos de puzzle, como caixas que precisam ser empurradas
- Zonas de perigo onde monstros podem detectar o jogador

A câmera acompanha o personagem durante a exploração, mantendo o foco no jogador enquanto ele avança pelo mapa.

## Mecânicas

### Mecânicas do Player
- **Walk** — movimentação para a esquerda e direita ao longo do cenário.
- **Run** — corrida para se deslocar mais rapidamente, fugir de ameaças ou atravessar trechos que exigem velocidade.
- **Jump** — pulo para alcançar plataformas em diferentes alturas e atravessar obstáculos.
- **Squat** — agachar para passar por espaços baixos, se esconder ou evitar obstáculos.
- **Push Objects** — empurrar caixas para resolver puzzles, abrir caminhos ou alcançar plataformas mais altas.
- **Death** — ao ser atingido por inimigos, cair em áreas perigosas ou falhar em desafios, uma animação de derrota é ativada e o jogador retorna ao último checkpoint ou reinicia a fase.

### Enemy
Os inimigos patrulham áreas fixas do cenário. Ao detectar o jogador dentro de sua zona de visão ou proximidade, iniciam perseguição. Se o jogador escapar da zona de detecção, o inimigo retorna ao patrulhamento.

### Interação com Objetos
O jogador pode interagir com objetos do cenário ao se aproximar deles: empurrar caixas, abrir portas, entrar em cavernas e ativar passagens escondidas. Cada objeto tem um comportamento específico que pode desbloquear novos caminhos ou levar o jogador para a próxima área.

### Plataformas Móveis
Algumas plataformas se movem em trajetos fixos, exigindo que o jogador calcule o momento certo para pular e se manter equilibrado. O jogador pode cair caso não sincronize seus movimentos com o deslocamento da plataforma.

### Armadilhas
Distribuídas pelos níveis, as armadilhas matam o jogador ao contato e exigem leitura do ambiente e timing para serem evitadas:

- **Arrow Trap** — disparador que lança flechas em intervalos fixos.
- **Saw Trap** — serra que se desloca em um trajeto horizontal ou vertical pré-definido.
- **Fire Trap** — chama que alterna entre ativa e inativa em ciclos.
- **Sandworm Trap** — criatura escondida que ataca quando o jogador entra em sua zona de detecção.

## Estrutura do Jogo

- **Estrutura de Fases:** o jogo é composto por fases lineares ambientadas em diferentes áreas da floresta. Cada fase possui obstáculos, inimigos e puzzles próprios, e termina quando o jogador encontra e utiliza o ponto de saída. A dificuldade aumenta progressivamente ao longo das fases.
- **Progressão:** conforme o jogador avança, os cenários se tornam mais sombrios e os desafios mais complexos, com inimigos mais presentes, puzzles mais elaborados e plataformas mais difíceis de atravessar.
- **Vitória:** o jogador vence ao encontrar o ponto de saída da última fase, escapando da floresta amaldiçoada.
- **Derrota:** o jogador perde ao ser atingido por inimigos, cair em áreas perigosas ou falhar em determinados desafios. Ao morrer, uma animação de derrota é ativada e o jogador reinicia a fase.

## Interação Player ↔ Game

- **Exploração e Navegação** — o jogador explora o cenário de forma linear, utilizando as mecânicas de movimentação para avançar pelas plataformas, desviar de obstáculos e encontrar o caminho até a saída da fase.
- **Confronto com Inimigos** — ao entrar na zona de detecção de um inimigo, o jogador é perseguido e deve escapar utilizando corrida, pulos ou se agachando. O contato com o inimigo resulta em morte e reinício da fase.
- **Resolução de Puzzles** — o jogador interage com objetos do cenário, como caixas e portas, para resolver puzzles que desbloqueiam novos caminhos. A solução exige observação do ambiente e uso correto das mecânicas disponíveis.
- **Feedback do Ambiente** — o cenário fornece pistas visuais e sonoras sobre perigos próximos, como a presença de inimigos ou áreas perigosas. O jogador deve estar atento aos sinais do ambiente para sobreviver.

## Interfaces

- **Menu Principal** — tela inicial com as opções *Iniciar Jogo*, *Configurações* e *Sair*. O visual segue o tema sombrio da floresta amaldiçoada.
- **HUD** — exibe as informações essenciais durante o jogo, como vidas e indicadores de status. O design é minimalista para não poluir a tela e manter a atmosfera de suspense.
- **Tela de Pause** — acessível durante o jogo, exibe as opções *Continuar*, *Reiniciar Fase* e *Voltar ao Menu Principal*.
- **Tela de Game Over** — exibida quando o jogador morre. Apresenta uma animação de derrota e as opções *Tentar Novamente* e *Voltar ao Menu Principal*.

## Estética

- **Visual:** estilo pixel art sombrio, com paleta de cores escuras e frias que reforçam a atmosfera de mistério e perigo da floresta. Efeitos de iluminação criam contraste entre áreas seguras e zonas de perigo.
- **Sonoro:** trilha sonora tensa e ambiente, criando uma sensação constante de alerta. Efeitos sonoros como passos, ruídos da floresta e sons dos inimigos reforçam a imersão e funcionam como pistas auditivas de perigo.

## Experiência Emocional

- **Tensão e Medo** — a presença constante de inimigos e a atmosfera sombria mantêm o jogador em estado de alerta durante toda a experiência.
- **Alívio** — ao escapar de um inimigo ou resolver um puzzle difícil, o jogador sente um alívio imediato, criando um ritmo emocional que mantém o engajamento.
- **Satisfação** — ao encontrar a saída de uma fase e avançar para a próxima área, o jogador sente satisfação pelo progresso conquistado.

## Habilidades Desenvolvidas pelo Jogador

- **Leitura do Ambiente** — identificar plataformas seguras, zonas de perigo e possíveis caminhos de fuga. O design dos níveis guia sutilmente o olhar do jogador para os elementos importantes.
- **Antecipação de Perigos** — antecipar a presença de inimigos e obstáculos através de pistas visuais e sonoras, como mudanças na iluminação, sons específicos e padrões de patrulha.
- **Noção de Progressão** — perceber claramente o avanço no jogo através das mudanças no cenário, que se torna progressivamente mais sombrio e desafiador.

## Como Jogar

### Requisitos
- **Unity 6000.3.12f1** (Unity 6) com suporte a build 2D.

### Executando o projeto
1. Clone o repositório.
2. Abra o Unity Hub, clique em *Add* e selecione a pasta raiz do projeto.
3. Abra o projeto na versão indicada acima.
4. Abra a cena `Assets/Scenes/MenuPrincipal.unity` e dê *Play*.

### Controles
| Ação | Tecla |
|------|-------|
| Mover | `A` / `D` ou setas esquerda/direita |
| Pular | `Espaço` |
| Agachar | `S` ou seta para baixo |
| Dash | `Shift esquerdo` |
| Wall Jump | `Espaço` enquanto desliza na parede |

## Equipe

- Anderson Julião
- Gabrielly Carneiro
- Iara Vivian
- Vinicius Cezimbra Miranda

## Tecnologia

Projeto desenvolvido em **Unity** (2D, Universal Render Pipeline), utilizando o novo Input System.
