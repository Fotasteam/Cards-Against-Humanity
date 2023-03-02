# Cards Against Humanity

[Cards Against Humanity](https://www.cardsagainsthumanity.com/) is a social game based-on a card game with the same title. 

## Information about the project:

### Copyright:

I do not own any rights to the card game. It is just a free-time project i've been working on for a while. The game is not that advanced. Wanted to post it on github, it's a great way to start learning UWP.

### The client:

Written in C# UWP using the [WinUI 2.8.2 Framework](https://microsoft.github.io/microsoft-ui-xaml/). The client also uses the [Microsoft UWP Toolkit](https://www.nuget.org/packages/Microsoft.Toolkit.Uwp/) and the [WatsonTCP](https://github.com/jchristn/WatsonTcp) for the TCP/IP connection.

### The server:

Written in .NET C# as a simple Console Application. It also uses the [WatsonTCP](https://github.com/jchristn/WatsonTcp) framework.

## Game rules:

### Game roles:

There are two types of players you can become:

- **The Card Czar**: Your job is to choose a card everyone has to answer.
- **Normal player**: Your job is to answer the Card Czars' card.

### How does the round playout:

1. Firstly, after everyone joins the server and the host starts the game, one of the players will be chosen as the Card Czar. Everyone has to wait until he chooses a card.
2. Next, every normal player will be given 10 random cards. They need to select the best answer.
3. After everyone has finally selected a card, everyone (including the Card Czar) needs to decide whose answer they like the most.
4. Lastly, the most-voted card will be revealed.

## How does it work:

The game works relatively simple. There is a variable called **gameState**, which helps the game understand what it has to do. Values for **gameState** are different for the server and the client.

### The client:

1. **gameState = 0** - Awaiting for clients. In this phase, the server waits for people to connect.
2. **gameState = 1** - Selecting the Card Czar. In this stage, the server selects a random player to become the Card Czar. Next, server sends a message to every client, sending player nicknames and telling them who is the Card Czar.
3. **gameState = 2** - Awaiting white cards (answer cards) from clients. The server waits for every normal player (not counting the Card Czar) to share the chosen white card. After everyone shared their white card, the server sends the list of white cards (and whose card it is) to everyone.
4. **gameState = 4** - Awaiting votes from clients. The server waits for everyone (including the Card Czar) to send a message telling which card they voted for. Afterwards, it sends this information to everyone else.

### The server:

