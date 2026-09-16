-
-
-

-
-
-

[![Wikipedia](https://en.wikipedia.org/static/images/mobile/copyright/wikipedia-wordmark-en-25.svg)![The Free Encyclopedia](https://en.wikipedia.org/static/images/mobile/copyright/wikipedia-tagline-en-25.svg)](https://en.wikipedia.org/wiki/Main_Page)

Search

-   [Donate](https://donate.wikimedia.org/?wmf_source=donate&wmf_medium=sidebar&wmf_campaign=en.wikipedia.org&uselang=en)
-   [Create account](https://en.wikipedia.org/w/index.php?title=Special:CreateAccount&returnto=Conway%27s+Game+of+Life "You are encouraged to create an account and log in; however, it is not mandatory")
-   [Log in](https://en.wikipedia.org/w/index.php?title=Special:UserLogin&returnto=Conway%27s+Game+of+Life "You're encouraged to log in; however, it's not mandatory. [ctrl-option-o]")

Contents
--------

hide

-   [(Top)
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#)
-   [Rules
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Rules)

-   [History
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#History)

-   [Analysis
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Analysis)
        -   [Pattern taxonomy
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Pattern_taxonomy)
                    -   [Oblique spaceships
                        ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Oblique_spaceships)

                    -   [Self-replication
                        ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Self-replication)

        -   [Computability theory
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Computability_theory)

        -   [Other disciplines
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Other_disciplines)

-   [Simulation
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Simulation)
        -   [History
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#History_2)

        -   [Algorithms
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Algorithms)

        -   [Modern programs
            ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Modern_programs)

-   [Variations
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Variations)

-   [In popular culture
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#In_popular_culture)

-   [See also
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#See_also)

-   [Notes
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Notes)

-   [References
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#References)

-   [External links
    ](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#External_links)

Conway's Game of Life
=====================

-   [Article](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life "View the content page [ctrl-option-c]")
-   [Talk](https://en.wikipedia.org/wiki/Talk:Conway%27s_Game_of_Life "Discuss improvements to the content page [ctrl-option-t]")

-   [Read](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)
-   [Edit](https://en.wikipedia.org/w/index.php?title=Conway%27s_Game_of_Life&action=edit "Edit this page [ctrl-option-e]")
-   [View history](https://en.wikipedia.org/w/index.php?title=Conway%27s_Game_of_Life&action=history "Past revisions of this page [ctrl-option-h]")

-
-
-
-

-

-

Appearance
hide
Text

-   Small
    Standard
    Large

Width

-   Standard
    Wide

Color

-   Automatic
    Light
    Dark

From Wikipedia, the free encyclopedia

"Conway game" redirects here. For Conway's surreal number game theory, see [Surreal number](https://en.wikipedia.org/wiki/Surreal_number "Surreal number").

[![](https://upload.wikimedia.org/wikipedia/commons/e/e5/Gospers_glider_gun.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Gospers_glider_gun.gif)

A single [Gosper](https://en.wikipedia.org/wiki/Bill_Gosper "Bill Gosper")'s [glider gun](https://en.wikipedia.org/wiki/Gun_(cellular_automaton) "Gun (cellular automaton)") creating [gliders](https://en.wikipedia.org/wiki/Glider_(Conway's_Life) "Glider (Conway's Life)")

[![](https://upload.wikimedia.org/wikipedia/commons/e/ec/Conways_game_of_life_breeder.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Conways_game_of_life_breeder.png)

A screenshot of a [puffer-type breeder](https://en.wikipedia.org/wiki/Puffer_train "Puffer train") (red) that leaves [glider guns](https://en.wikipedia.org/wiki/Gun_(cellular_automaton) "Gun (cellular automaton)") (green) in its wake, which in turn create gliders (blue) ([animation](https://en.wikipedia.org/wiki/File:Conways_game_of_life_breeder_animation.gif "File:Conways game of life breeder animation.gif"))

The **Game of Life**, also known as **Conway's Game of Life** (sometimes abbreviated as **CGoL**) or simply **Life**, is a [cellular automaton](https://en.wikipedia.org/wiki/Cellular_automaton "Cellular automaton") devised by the British [mathematician](https://en.wikipedia.org/wiki/Mathematician "Mathematician") [John Horton Conway](https://en.wikipedia.org/wiki/John_Horton_Conway "John Horton Conway") in 1970.^[\[1\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-:0-1)^ It is a [zero-player game](https://en.wikipedia.org/wiki/Zero-player_game "Zero-player game"),^[\[2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-bcg-2)^ meaning that its evolution is determined by its initial state, requiring no further input. One interacts with the Game of Life by creating an initial configuration and observing how it evolves in discrete time steps called generations. There is no limit to the number of generations (computational resources permitting) nor is there a win condition.

The Game of Life is played on an infinite square grid, with each cell being in one of two states: live or dead. Cells in configurations called *patterns* evolve over generations according to the number of live and dead cells in their [Moore neighborhood](https://en.wikipedia.org/wiki/Moore_neighborhood "Moore neighborhood"), that is, the eight cells in their immediate proximity.

Patterns can be grouped into different types, such as *[still lifes](https://en.wikipedia.org/wiki/Still_life_(cellular_automaton) "Still life (cellular automaton)")*, *[oscillators](https://en.wikipedia.org/wiki/Oscillator_(cellular_automaton) "Oscillator (cellular automaton)")*, and *[spaceships](https://en.wikipedia.org/wiki/Spaceship_(cellular_automaton) "Spaceship (cellular automaton)")*.

The Game of Life was first simulated manually, with computerized simulations arriving soon afterwards. Nowadays, more modern programs such as [Golly](https://en.wikipedia.org/wiki/Golly_(program) "Golly (program)") are used. These programs often do not store cells as two-dimensional arrays, instead using algorithms such as [Hashlife](https://en.wikipedia.org/wiki/Hashlife "Hashlife") which represent patterns as a [tree](https://en.wikipedia.org/wiki/Tree_(abstract_data_type) "Tree (abstract data type)") structure.

The Game of Life has spawned a number of other cellular automata, known as [Life-like cellular automata](https://en.wikipedia.org/wiki/Life-like_cellular_automata "Life-like cellular automata"). Examples include [Highlife](https://en.wikipedia.org/wiki/Highlife_(cellular_automaton) "Highlife (cellular automaton)") and [Seeds](https://en.wikipedia.org/wiki/Seeds_(cellular_automaton) "Seeds (cellular automaton)"). Other variations may include more than two states or use a non-square grid.

Rules
-----

The universe of the Game of Life is [an infinite, two-dimensional orthogonal grid of square](https://en.wikipedia.org/wiki/Square_tiling "Square tiling") *cells*, each of which is in one of two possible states, *live* or *dead* (or *populated* and *unpopulated*, respectively). Every cell interacts with its eight *neighbours* (its [Moore neighborhood](https://en.wikipedia.org/wiki/Moore_neighborhood "Moore neighborhood")), which are the cells that are horizontally, vertically, or diagonally adjacent. At each step in time, the following transitions occur:

1.  Any live cell with fewer than two live neighbours dies, as if by underpopulation.
2.  Any live cell with two or three live neighbours lives on to the next generation.
3.  Any live cell with more than three live neighbours dies, as if by overpopulation.
4.  Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.^[\[3\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-3)^^[\[4\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-lifebook-4)^^: 3^ 

This makes it described by the *rulestring* B3/S23,^[\[nb 1\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-5)^ where the numbers before the slash signify conditions for dead cells becoming alive, and the ones after it signifying survival conditions for cells that are already alive.^[\[4\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-lifebook-4)^^: 5^ 

The initial pattern constitutes the *seed* of the system. The first *generation* is created by applying the above rules simultaneously to every cell in the seed, live or dead; births and deaths occur simultaneously, and the discrete moment at which this happens is sometimes called a *tick*.^[\[nb 2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-9)^ Each generation is a *[pure function](https://en.wikipedia.org/wiki/Pure_function "Pure function")* of the preceding one. The rules continue to be applied repeatedly to create further generations.

In the editable widget below, one of the patterns discovered by Conway, the I-heptomino can be simulated.

![Zoom in](https://upload.wikimedia.org/wikipedia/commons/2/2e/WikiWidgetZoomInButton.png "Zoom in")![Zoom out](https://upload.wikimedia.org/wikipedia/commons/6/63/WikiWidgetZoomOutButton.png "Zoom out")![Grid](https://upload.wikimedia.org/wikipedia/commons/a/a9/WikiWidgetGridButton.png "Grid")![Reset](https://upload.wikimedia.org/wikipedia/commons/0/0e/WikiWidgetResetButton.png "Reset")![Play](https://upload.wikimedia.org/wikipedia/commons/b/b8/WikiWidgetPlayButton.png "Play")![Next generation](https://upload.wikimedia.org/wikipedia/commons/b/bf/WikiWidgetNextFrameButton.png "Next generation")
Generation 0Population 7

History
-------

See also: [Cellular automaton § History](https://en.wikipedia.org/wiki/Cellular_automaton#History "Cellular automaton")

Cellular automata have their origins in the work of [Stanisław Ulam](https://en.wikipedia.org/wiki/Stanis%C5%82aw_Ulam "Stanisław Ulam") and [John von Neumann](https://en.wikipedia.org/wiki/John_von_Neumann "John von Neumann") in the 1940s. Development continued through the 1950s and 1960s when, in 1968, [John Horton Conway](https://en.wikipedia.org/wiki/John_Horton_Conway "John Horton Conway") began doing experiments with a variety of different two-dimensional cellular automaton rules. Conway's initial goal was to define an interesting and unpredictable cellular automaton. According to [Martin Gardner](https://en.wikipedia.org/wiki/Martin_Gardner "Martin Gardner"), Conway experimented with different rules, aiming for a ruleset that would allow for patterns to "apparently" grow without limit, while keeping it difficult to *prove* that any given pattern would do so. Moreover, some "simple initial patterns" should "grow and change for a considerable period of time" before settling into a static configuration or a repeating loop.^[\[1\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-:0-1)^ Conway later wrote that the basic motivation for Life was to create a "universal" cellular automaton.^[\[8\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-10)^^\[*[better source needed](https://en.wikipedia.org/wiki/Wikipedia:Verifiability#Questionable_sources "Wikipedia:Verifiability")*\]^

The game made its first public appearance in the October 1970 issue of *[Scientific American](https://en.wikipedia.org/wiki/Scientific_American "Scientific American")*, in [Martin Gardner](https://en.wikipedia.org/wiki/Martin_Gardner "Martin Gardner")'s "[Mathematical Games](https://en.wikipedia.org/wiki/Mathematical_Games_(column) "Mathematical Games (column)")" column, which was based on personal conversations with Conway. Theoretically, the Game of Life has the power of a [universal Turing machine](https://en.wikipedia.org/wiki/Universal_Turing_machine "Universal Turing machine"): anything that can be computed [algorithmically](https://en.wikipedia.org/wiki/Algorithm "Algorithm") can be computed within the Game of Life.^[\[9\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-chapman-11)^^[\[2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-bcg-2)^ Gardner wrote, "Because of Life's analogies with the rise, fall, and alterations of a society of living organisms, it belongs to a growing class of what are called 'simulation games' (games that resemble real-life processes)."^[\[1\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-:0-1)^

The popularity of the Game of Life was helped by its coming into being at the same time as increasingly inexpensive computer access. The game could be run for hours on these machines, which would otherwise have remained unused at night. In this respect, it foreshadowed the later popularity of computer-generated [fractals](https://en.wikipedia.org/wiki/Fractal "Fractal"). For many, the Game of Life was simply a programming challenge: a fun way to use otherwise wasted [CPU](https://en.wikipedia.org/wiki/Central_processing_unit "Central processing unit") cycles. It developed a cult following through the 1970s and beyond; current developments have gone so far as to create theoretic emulations of computer systems within the confines of a Game of Life board.^[\[10\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-12)^

Analysis
--------

### Pattern taxonomy

Many different types of patterns occur in the Game of Life, which are classified according to their behaviour. Common pattern types include *[still lifes](https://en.wikipedia.org/wiki/Still_life_(cellular_automaton) "Still life (cellular automaton)")*, which do not change from one generation to the next; *[oscillators](https://en.wikipedia.org/wiki/Oscillator_(cellular_automaton) "Oscillator (cellular automaton)")*, which return to their initial state after a finite number of generations; and *[spaceships](https://en.wikipedia.org/wiki/Spaceship_(cellular_automaton) "Spaceship (cellular automaton)")*, which translate themselves across the grid.^[\[4\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-lifebook-4)^^: 4, 6^ 

The earliest interesting patterns in the Game of Life were discovered without the use of computers. The simplest still lifes and oscillators were discovered while tracking the fates of various small starting configurations using [graph paper](https://en.wikipedia.org/wiki/Graph_paper "Graph paper"), [blackboards](https://en.wikipedia.org/wiki/Blackboard "Blackboard"), and physical game boards, such as those used in [Go](https://en.wikipedia.org/wiki/Go_(board_game) "Go (board game)"). During this early research, Conway discovered that the R-[pentomino](https://en.wikipedia.org/wiki/Pentomino "Pentomino") failed to stabilize in a small number of generations. In fact, it takes 1103 generations to stabilize, by which time it has a population of 116 and has generated six escaping [gliders](https://en.wikipedia.org/wiki/Glider_(Conway's_Life) "Glider (Conway's Life)"); these were the first spaceships ever discovered.^[\[11\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-13)^

Frequently occurring^[\[12\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-14)^^[\[13\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-15)^ examples (in that they emerge frequently from a random starting configuration of cells called *soups*^[\[4\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-lifebook-4)^^: 5^ ) of the three aforementioned pattern types are shown below, with live cells shown in black and dead cells in white. *Period* refers to the number of ticks a pattern must iterate through before returning to its initial configuration.

‹ The [template](https://en.wikipedia.org/wiki/Help:Template "Help:Template") below (*[Col-begin](https://en.wikipedia.org/wiki/Template:Col-begin "Template:Col-begin")*) is being considered for deletion. See [templates for discussion](https://en.wikipedia.org/wiki/Wikipedia:Templates_for_discussion/Log/2026_September_15#Template:Col-begin "Wikipedia:Templates for discussion/Log/2026 September 15") to help reach a consensus. ›

| \| Still lifes \|\|\| --- \| --- \|\| Block \| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/9/96/Game_of_life_block_with_border.svg/120px-Game_of_life_block_with_border.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_block_with_border.svg) \|\| Bee-hive \| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/6/67/Game_of_life_beehive.svg/120px-Game_of_life_beehive.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_beehive.svg) \|\| Loaf \| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/f/f4/Game_of_life_loaf.svg/120px-Game_of_life_loaf.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_loaf.svg) \|\| Boat \| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/7/7f/Game_of_life_boat.svg/120px-Game_of_life_boat.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_boat.svg) \|\| Tub \| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/3/31/Game_of_life_flower.svg/120px-Game_of_life_flower.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_flower.svg) \| | \| Oscillators \|\|\| --- \| --- \|\| Blinker(period 2) \| [![](https://upload.wikimedia.org/wikipedia/commons/9/95/Game_of_life_blinker.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_blinker.gif) \|\| Toad(period 2) \| [![](https://upload.wikimedia.org/wikipedia/commons/1/12/Game_of_life_toad.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_toad.gif) \|\| Beacon(period 2) \| [![](https://upload.wikimedia.org/wikipedia/commons/1/1c/Game_of_life_beacon.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_beacon.gif) \|\| Pulsar(period 3) \| [![](https://upload.wikimedia.org/wikipedia/commons/0/07/Game_of_life_pulsar.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_pulsar.gif) \|\| Penta-decathlon(period 15) \| [![](https://upload.wikimedia.org/wikipedia/commons/f/fb/I-Column.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:I-Column.gif) \| | \| Spaceships \|\|\| --- \| --- \|\| Glider \| [![](https://upload.wikimedia.org/wikipedia/commons/f/f2/Game_of_life_animated_glider.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_animated_glider.gif) \|\| Light-weightspaceship(LWSS) \| [![](https://upload.wikimedia.org/wikipedia/commons/3/37/Game_of_life_animated_LWSS.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Game_of_life_animated_LWSS.gif) \|\| Middle-weightspaceship(MWSS) \| [![](https://upload.wikimedia.org/wikipedia/commons/4/4e/Animated_Mwss.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Animated_Mwss.gif) \|\| Heavy-weightspaceship(HWSS) \| [![](https://upload.wikimedia.org/wikipedia/commons/4/4f/Animated_Hwss.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Animated_Hwss.gif) \| |
| --- |  --- |  --- |

The *pulsar*^[\[14\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-16)^ is the most common period-3 oscillator.^[\[15\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-17)^ The great majority of naturally occurring oscillators have a period of 2, like the blinker and the toad, but oscillators of all periods are known to exist,^[\[16\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-18)^^[\[17\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-19)^ and oscillators of periods 4, 8, 14, 15, 30, and a few others have been seen to arise from random initial conditions.^[\[18\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-20)^ Patterns which evolve for long periods before stabilizing are called *[methuselahs](https://en.wikipedia.org/wiki/Methuselah_(cellular_automaton) "Methuselah (cellular automaton)")*, the first-discovered of which was the R-pentomino. *Diehard* is a pattern that disappears after 130 generations. Starting patterns of eight or more cells can be made to die after an arbitrarily long time.^[\[19\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-21)^ *Acorn* takes 5,206 generations to generate 633 cells, including 13 escaped gliders.^[\[20\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-22)^

| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/1/1c/Game_of_life_fpento.svg/120px-Game_of_life_fpento.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_fpento.svg)The R-pentomino | [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/9/99/Game_of_life_diehard.svg/250px-Game_of_life_diehard.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_diehard.svg)Diehard | [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/b/b9/Game_of_life_acorn.svg/250px-Game_of_life_acorn.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_acorn.svg)Acorn |
| --- |  --- |  --- |

Conway originally conjectured that no pattern can grow indefinitely---i.e. that for any initial configuration with a finite number of living cells, the population cannot grow beyond some finite upper limit. In the game's original appearance in "Mathematical Games", Conway offered a prize of fifty dollars (equivalent to $415 in 2025) to the first person who could prove or disprove the conjecture before the end of 1970. The prize was won in November by a team from the [Massachusetts Institute of Technology](https://en.wikipedia.org/wiki/Massachusetts_Institute_of_Technology "Massachusetts Institute of Technology"), led by [Bill Gosper](https://en.wikipedia.org/wiki/Bill_Gosper "Bill Gosper"); the "Gosper glider gun" produces its first glider on the 15th generation, and another glider every 30th generation from then on. For many years, this glider gun was the smallest one known.^[\[21\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-23)^ In 2015, a gun called the "Simkin glider gun", which releases a glider every 120th generation, was discovered that has fewer live cells but which is spread out across a larger bounding box at its extremities.^[\[22\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-24)^

| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/e/e0/Game_of_life_glider_gun.svg/500px-Game_of_life_glider_gun.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_glider_gun.svg)Gosper glider gun |
| --- |
| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/6/64/Game_of_life_Simkin_glider_gun.svg/500px-Game_of_life_Simkin_glider_gun.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_Simkin_glider_gun.svg)Simkin glider gun |

Smaller patterns were later found that also exhibit infinite growth. All three of the patterns shown below grow indefinitely. The first two create a single *block-laying switch engine*: a configuration that leaves behind two-by-two still life blocks as it translates itself across the game's universe. The third configuration creates two such patterns. The first has only ten live cells, which has been proven to be minimal.^[\[23\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-25)^ The second fits in a five-by-five square, and the third is only one cell high.

| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/7/72/Game_of_life_infinite1.svg/250px-Game_of_life_infinite1.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_infinite1.svg) [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/a/ae/Game_of_life_infinite2.svg/120px-Game_of_life_infinite2.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_infinite2.svg) |
| --- |
| [![](https://thumb.wikimedia.org/wikipedia/commons/thumb/9/95/Game_of_life_infinite3.svg/960px-Game_of_life_infinite3.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Game_of_life_infinite3.svg) |

Later discoveries included other *[guns](https://en.wikipedia.org/wiki/Gun_(cellular_automaton) "Gun (cellular automaton)")*, which are stationary, and which produce gliders or other spaceships; *[puffer trains](https://en.wikipedia.org/wiki/Puffer_train "Puffer train")*, which move along leaving behind a trail of debris; and *[rakes](https://en.wikipedia.org/wiki/Rake_(cellular_automaton) "Rake (cellular automaton)")*, which move and emit spaceships.^[\[24\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-26)^ Gosper also constructed the first pattern with an [asymptotically optimal](https://en.wikipedia.org/wiki/Asymptotically_optimal_algorithm "Asymptotically optimal algorithm") [quadratic growth rate](https://en.wikipedia.org/wiki/Quadratic_growth "Quadratic growth"), called a *[breeder](https://en.wikipedia.org/wiki/Breeder_(cellular_automaton) "Breeder (cellular automaton)")*, which worked by leaving behind a trail of guns.^[\[25\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-27)^

It is possible for gliders to interact with other objects in interesting ways. For example, if two gliders are shot at a block in a specific position, the block will move closer to the source of the gliders. If three gliders are shot in just the right way, the block will move farther away. This *sliding block memory* can be used to simulate a [counter](https://en.wikipedia.org/wiki/Counter_(digital) "Counter (digital)"). It is possible to construct [logic gates](https://en.wikipedia.org/wiki/Logic_gate "Logic gate") such as *[AND](https://en.wikipedia.org/wiki/Logical_conjunction "Logical conjunction")*, *[OR](https://en.wikipedia.org/wiki/Logical_disjunction "Logical disjunction")*, and *[NOT](https://en.wikipedia.org/wiki/Negation "Negation")* using gliders. It is possible to build a pattern that acts like a [finite-state machine](https://en.wikipedia.org/wiki/Finite-state_machine "Finite-state machine") connected to two counters. This has the same computational power as a [universal Turing machine](https://en.wikipedia.org/wiki/Universal_Turing_machine "Universal Turing machine"), so the Game of Life is theoretically as powerful as any computer with unlimited memory and no time constraints; it is [Turing complete](https://en.wikipedia.org/wiki/Turing_complete "Turing complete").^[\[9\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-chapman-11)^^[\[2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-bcg-2)^ In fact, several different programmable computer architectures^[\[26\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-28)^^[\[27\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-29)^ have been implemented in the Game of Life, including a pattern that simulates [Tetris](https://en.wikipedia.org/wiki/Tetris "Tetris").^[\[28\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-30)^

#### Oblique spaceships

Until the 2010s, all known spaceships could only move orthogonally or diagonally. Spaceships which move neither orthogonally nor diagonally are commonly referred to as *oblique spaceships*.^[\[29\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-31)^ On May 18, 2010, Andrew J. Wade announced the first oblique spaceship, dubbed "Gemini", that creates a copy of itself on (5,1) further while destroying its parent.^[\[30\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-32)^ This pattern replicates in 34 million generations, and uses an instruction tape made of gliders oscillating between two stable configurations made of Chapman--Greene construction arms. These, in turn, create new copies of the pattern, and destroy the previous copy. In December 2015, diagonal versions of the Gemini were built.^[\[31\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-33)^

A more specific case is a *knightship*, a spaceship that moves two squares left for every one square it moves down (like a [knight in chess](https://en.wikipedia.org/wiki/Knight_(chess) "Knight (chess)")), whose existence had been predicted by [Elwyn Berlekamp](https://en.wikipedia.org/wiki/Elwyn_Berlekamp "Elwyn Berlekamp") since 1982. The first elementary knightship, Sir Robin, was discovered in 2018 by Adam P. Goucher.^[\[32\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-34)^ This is the first new spaceship movement pattern for an elementary spaceship found in forty-eight years. "Elementary" means that it cannot be decomposed into smaller interacting patterns such as gliders and still lifes.^[\[33\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-35)^

#### Self-replication

A pattern can contain a collection of guns that fire gliders in such a way as to construct new objects, including copies of the original pattern. A *universal constructor* can be built which contains a Turing complete computer, and which can build many types of complex objects, including more copies of itself.^[\[2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-bcg-2)^ On November 23, 2013, Dave Greene built the first [replicator](https://en.wikipedia.org/wiki/Replicator_(cellular_automaton) "Replicator (cellular automaton)") in the Game of Life that creates a complete copy of itself, including the instruction tape.^[\[34\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-36)^ In October 2018, Adam P. Goucher finished his construction of the 0E0P metacell, a metacell capable of self-replication. This differed from previous metacells, such as the OTCA metapixel by Brice Due, which only worked with already constructed copies near them. The 0E0P metacell works by using construction arms to create copies that simulate the programmed rule.^[\[35\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-37)^ The actual simulation of the Game of Life or other [Moore neighbourhood](https://en.wikipedia.org/wiki/Moore_neighbourhood "Moore neighbourhood") rules is done by simulating an equivalent rule using the [von Neumann neighbourhood](https://en.wikipedia.org/wiki/Von_Neumann_neighbourhood "Von Neumann neighbourhood") with more states.^[\[36\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-38)^ The name 0E0P is short for "Zero Encoded by Zero Population", which indicates that instead of a metacell being in an "off" state simulating empty space, the 0E0P metacell removes itself when the cell enters that state, leaving a blank space.^[\[37\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-39)^

### Computability theory

Many starting patterns in the Game of Life eventually become a combination of still lifes, oscillators, and spaceships; other patterns may be called chaotic. A pattern may stay chaotic for a very long time until it eventually settles to such a combination.

Patterns may be constructed to emulate logical gates and replicate computing components such as information storage, logical instruction sets and program storage. These components then can be used to construct a computer within the Game of Life. Therefore, the Game of Life is Turing-complete and may therefore execute arbitrary programs. By the [halting problem](https://en.wikipedia.org/wiki/Halting_problem "Halting problem") it is [undecidable](https://en.wikipedia.org/wiki/Undecidable_problem "Undecidable problem") whether an arbitrary program executed in the Game of Life would ever terminate. Translating back into the terms of the Game of Life, given an initial configuration pattern and a target pattern, it is also undecidable whether the target pattern will ever appear.^[\[2\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-bcg-2)^

### Other disciplines

Since its publication, the Game of Life has attracted much interest because of the surprising ways in which the patterns can evolve. It provides an example of [emergence](https://en.wikipedia.org/wiki/Emergence "Emergence") and [self-organization](https://en.wikipedia.org/wiki/Self-organization "Self-organization"). A version of Life that incorporates random fluctuations has been used in [physics](https://en.wikipedia.org/wiki/Physics "Physics") to study [phase transitions](https://en.wikipedia.org/wiki/Phase_transition "Phase transition") and [nonequilibrium dynamics](https://en.wikipedia.org/wiki/Nonequilibrium_statistical_mechanics "Nonequilibrium statistical mechanics").^[\[38\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-40)^ The game can also serve as a didactic [analogy](https://en.wikipedia.org/wiki/Analogy "Analogy"), used to convey the somewhat counter-intuitive notion that design and organization can spontaneously emerge in the absence of a designer. For example, philosopher [Daniel Dennett](https://en.wikipedia.org/wiki/Daniel_Dennett "Daniel Dennett") has used the analogy of the Game of Life "universe" extensively to illustrate the possible evolution of complex philosophical constructs, such as [consciousness](https://en.wikipedia.org/wiki/Consciousness "Consciousness") and [free will](https://en.wikipedia.org/wiki/Free_will "Free will"), from the relatively simple set of deterministic physical laws which might govern our universe.^[\[39\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-41)^^[\[40\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-42)^^[\[41\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-43)^

From most random initial patterns of living cells on the grid, observers will find the population constantly changing as the generations tick by. The patterns that emerge from the simple rules may be considered a form of [mathematical beauty](https://en.wikipedia.org/wiki/Mathematical_beauty "Mathematical beauty"). Small isolated subpatterns with no initial symmetry tend to become symmetrical. Once this happens, the symmetry may increase in richness, but it cannot be lost unless a nearby subpattern comes close enough to disturb it. In a very few cases, the society eventually dies out, with all living cells vanishing, though this may not happen for a great many generations. Most initial patterns eventually burn out, producing either stable figures or patterns that oscillate forever between two or more states;^[\[42\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-44)^^[\[43\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-45)^ many also produce one or more gliders or spaceships that travel indefinitely away from the initial location. Because of the nearest-neighbour based rules, no information can travel through the grid at a greater rate than one cell per unit time, so this velocity is said to be the [cellular automaton speed of light](https://en.wikipedia.org/wiki/Speed_of_light_(cellular_automaton) "Speed of light (cellular automaton)") and denoted *c*.

Simulation
----------

### History

[![](https://thumb.wikimedia.org/wikipedia/commons/thumb/0/05/Turing_Machine_in_Golly.png/330px-Turing_Machine_in_Golly.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Turing_Machine_in_Golly.png)

The 6366548773467669985195496000th (6×10^27^) generation of a [Turing machine](https://en.wikipedia.org/wiki/Turing_machine "Turing machine"), made in the Game of Life, computed in less than 30 seconds on an [Intel Core](https://en.wikipedia.org/wiki/Intel_Core_2 "Intel Core 2") Duo 2 GHz CPU using Golly in [Hashlife](https://en.wikipedia.org/wiki/Hashlife "Hashlife") mode

Computers have been used to follow and simulate the Game of Life since it was first publicized. When John Conway was first investigating how various starting configurations developed, he tracked them by hand using a [go](https://en.wikipedia.org/wiki/Go_(game) "Go (game)") board with its black and white stones. This was tedious and prone to errors. The first interactive Game of Life program was written in an early version of [ALGOL 68C](https://en.wikipedia.org/wiki/ALGOL_68C "ALGOL 68C") for the [PDP-7](https://en.wikipedia.org/wiki/PDP-7 "PDP-7") by [M. J. T. Guy](https://en.wikipedia.org/wiki/Michael_Guy_(computer_scientist) "Michael Guy (computer scientist)") and [S. R. Bourne](https://en.wikipedia.org/wiki/Stephen_R._Bourne "Stephen R. Bourne"). The results were published in the October 1970 issue of *[Scientific American](https://en.wikipedia.org/wiki/Scientific_American "Scientific American")*, along with the statement: "Without its help, some discoveries about the game would have been difficult to make."^[\[1\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-:0-1)^

A color version of the Game of Life was written by Ed Hall in 1976 for [Cromemco](https://en.wikipedia.org/wiki/Cromemco "Cromemco") microcomputers, and a display from that program filled the cover of the June 1976 issue of *[Byte](https://en.wikipedia.org/wiki/Byte_(magazine) "Byte (magazine)")*.^[\[44\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-46)^ The advent of microcomputer-based color graphics from Cromemco has been credited with a revival of interest in the game.^[\[45\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-47)^

Two early implementations of the Game of Life on home computers were by Malcolm Banthorpe written in [BBC BASIC](https://en.wikipedia.org/wiki/BBC_BASIC "BBC BASIC"). The first was in the January 1984 issue of *[Acorn User](https://en.wikipedia.org/wiki/Acorn_User "Acorn User")* magazine, and Banthorpe followed this with a three-dimensional version in the May 1984 issue.^[\[46\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-48)^ Susan Stepney, Professor of Computer Science at the [University of York](https://en.wikipedia.org/wiki/University_of_York "University of York"), followed this up in 1988 with Life on the Line, a program that generated one-dimensional cellular automata.^[\[47\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-49)^

### Algorithms

Early patterns with unknown futures, such as the R-pentomino, led computer programmers to write programs to track the evolution of patterns in the Game of Life. Most of the early [algorithms](https://en.wikipedia.org/wiki/Algorithm "Algorithm") were similar: they represented the patterns as two-dimensional arrays in computer memory. Typically, two arrays are used: one to hold the current generation, and one to calculate its successor. Often 0 and 1 represent dead and live cells, respectively. A nested [for loop](https://en.wikipedia.org/wiki/For_loop "For loop") considers each element of the current array in turn, counting the live neighbours of each cell to decide whether the corresponding element of the successor array should be 0 or 1. The successor array is displayed. For the next iteration, the arrays may swap roles so that the successor array in the last iteration becomes the current array in the next iteration, or one may copy the values of the second array into the first array then update the second array from the first array again.

A variety of minor enhancements to this basic scheme are possible, and there are many ways to save unnecessary computation. A cell that did not change at the last time step, and none of whose neighbours changed, is guaranteed not to change at the current time step as well, so a program that keeps track of which areas are active can save time by not updating inactive zones.^[\[48\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-50)^

[![Game of Life on the surface of a trefoil knot](https://thumb.wikimedia.org/wikipedia/commons/thumb/6/64/Trefoil_knot_conways_game_of_life.gif/250px-Trefoil_knot_conways_game_of_life.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Trefoil_knot_conways_game_of_life.gif)

The Game of Life on the surface of a [toroidal](https://en.wikipedia.org/wiki/Torus "Torus") [trefoil knot](https://en.wikipedia.org/wiki/Trefoil_knot#In_religion_and_culture "Trefoil knot")

To avoid decisions and branches in the counting loop, the rules can be rearranged from an [egocentric](https://en.wikipedia.org/wiki/Egocentrism "Egocentrism") approach of the inner field regarding its neighbours to a scientific observer's viewpoint: if the sum of all nine fields in a given neighbourhood is three, the inner field state for the next generation will be life; if the all-field sum is four, the inner field retains its current state; and every other sum sets the inner field to death.

To save memory, the storage can be reduced to one array plus two line buffers. One line buffer is used to calculate the successor state for a line, then the second line buffer is used to calculate the successor state for the next line. The first buffer is then written to its line and freed to hold the successor state for the third line. If a [toroidal](https://en.wikipedia.org/wiki/Torus "Torus") array is used, a third buffer is needed so that the original state of the first line in the array can be saved until the last line is computed.

[![](https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d1/Long_gun.gif/250px-Long_gun.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Long_gun.gif)

Glider gun within a toroidal array. The stream of gliders eventually wraps around and destroys the gun.

[![](https://thumb.wikimedia.org/wikipedia/commons/thumb/1/18/%D0%98%D0%B3%D1%80%D0%B0_%22%D0%96%D0%B8%D0%B7%D0%BD%D1%8C%22.gif/250px-%D0%98%D0%B3%D1%80%D0%B0_%22%D0%96%D0%B8%D0%B7%D0%BD%D1%8C%22.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:%D0%98%D0%B3%D1%80%D0%B0_%22%D0%96%D0%B8%D0%B7%D0%BD%D1%8C%22.gif)

Red glider on a square lattice with periodic boundary conditions

In principle, the Game of Life field is infinite, but computers have finite memory. This leads to problems when the active area encroaches on the border of the array. Programmers have used several strategies to address these problems. The simplest strategy is to assume that every cell outside the array is dead. This is easy to program but leads to inaccurate results when the active area crosses the boundary. A more sophisticated trick is to consider the left and right edges of the field to be stitched together, and the top and bottom edges also, yielding a [toroidal](https://en.wikipedia.org/wiki/Torus "Torus") array. The result is that active areas that move across a field edge reappear at the opposite edge. Inaccuracy can still result if the pattern grows too large, but there are no pathological edge effects. Techniques of dynamic storage allocation may also be used, creating ever-larger arrays to hold growing patterns. The Game of Life on a finite field is sometimes explicitly studied; some implementations, such as *[Golly](https://en.wikipedia.org/wiki/Golly_(program) "Golly (program)")*, support a choice of the standard infinite field, a field infinite only in one dimension, or a finite field, with a choice of topologies such as a cylinder, a torus, or a [Möbius strip](https://en.wikipedia.org/wiki/M%C3%B6bius_strip "Möbius strip").

Alternatively, programmers may abandon the notion of representing the Game of Life field with a two-dimensional array, and use a different data structure, such as a vector of coordinate pairs representing live cells. This allows the pattern to move about the field unhindered, as long as the population does not exceed the size of the live-coordinate array. The drawback is that counting live neighbours becomes a hash-table lookup or search operation, slowing down simulation speed. With more sophisticated data structures this problem can also be largely solved.^\[*[citation needed](https://en.wikipedia.org/wiki/Wikipedia:Citation_needed "Wikipedia:Citation needed")*\]^

For exploring large patterns at great time depths, sophisticated algorithms such as [Hashlife](https://en.wikipedia.org/wiki/Hashlife "Hashlife") may be useful. There is also a method for implementation of the Game of Life and other cellular automata using arbitrary asynchronous updates while still exactly [emulating](https://en.wikipedia.org/wiki/Emulator "Emulator") the behaviour of the synchronous game.^[\[49\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-51)^ [Source code](https://en.wikipedia.org/wiki/Source_code "Source code") examples that implement the basic Game of Life scenario in various programming languages, including [C](https://en.wikipedia.org/wiki/C_(programming_language) "C (programming language)"), [C++](https://en.wikipedia.org/wiki/C++ "C++"), [Java](https://en.wikipedia.org/wiki/Java_(programming_language) "Java (programming language)") and [Python](https://en.wikipedia.org/wiki/Python_(programming_language) "Python (programming language)") can be found at [Rosetta Code](https://en.wikipedia.org/wiki/Rosetta_Code "Rosetta Code").^[\[50\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-52)^

### Modern programs

There are now thousands of Game of Life programs online, so a full list will not be provided here. The following is a small selection of programs with some special claim to notability, such as popularity or unusual features. Most of these programs incorporate a [graphical user interface](https://en.wikipedia.org/wiki/Graphical_user_interface "Graphical user interface") for pattern editing and simulation, the capability for simulating multiple rules including the Game of Life, and a large library of interesting patterns in the Game of Life and other cellular automaton rules.

-   [Golly](https://en.wikipedia.org/wiki/Golly_(program) "Golly (program)") is a cross-platform ([Windows](https://en.wikipedia.org/wiki/Microsoft_Windows "Microsoft Windows"), [MacOS](https://en.wikipedia.org/wiki/MacOS "MacOS"), [Linux](https://en.wikipedia.org/wiki/Linux "Linux"), [iOS](https://en.wikipedia.org/wiki/IOS "IOS"), and [Android](https://en.wikipedia.org/wiki/Android_(operating_system) "Android (operating system)")) open-source simulation system for the Game of Life and other cellular automata (including all Life-like cellular automata, the Generations family of cellular automata from Mirek's Cellebration, and John von Neumann's 29-state cellular automaton) built by Andrew Trevorrow and Tomas Rokicki. It includes the Hashlife algorithm for extremely fast generation, and [Lua](https://en.wikipedia.org/wiki/Lua "Lua") or [Python](https://en.wikipedia.org/wiki/Python_(programming_language) "Python (programming language)") scriptability for both editing and simulation.
-   Mirek's Cellebration is a freeware one- and two-dimensional cellular automata viewer, explorer, and editor for Windows. It includes powerful facilities for simulating and viewing a wide variety of cellular automaton rules, including the Game of Life, and a scriptable editor.
-   Xlife is a cellular-automaton laboratory by Jon Bennett. The standard UNIX X11 Game of Life simulation application for a long time, it has also been ported to Windows. It can handle cellular automaton rules with the same neighbourhood as the Game of Life, and up to eight possible states per cell.^[\[51\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-53)^

Variations
----------

Main article: [Life-like cellular automaton](https://en.wikipedia.org/wiki/Life-like_cellular_automaton "Life-like cellular automaton")

Since the Game of Life's inception, new, similar cellular automata have been developed. The standard Game of Life is symbolized in rule-string notation as B3/S23. A cell is born if it has exactly three neighbours, survives if it has two or three living neighbours, and dies otherwise. The first number, or list of numbers, is what is required for a dead cell to be born. The second set is the requirement for a live cell to survive to the next generation. Hence B6/S16 means "a cell is born if there are six neighbours, and lives on if there are either one or six neighbours". Cellular automata on a two-dimensional grid that can be described in this way are known as [Life-like cellular automata](https://en.wikipedia.org/wiki/Life-like_cellular_automaton "Life-like cellular automaton"). Another common Life-like automaton, [Highlife](https://en.wikipedia.org/wiki/Highlife_(cellular_automaton) "Highlife (cellular automaton)"), is described by the rule B36/S23, because having six neighbours, in addition to the original game's B3/S23 rule, causes a birth. HighLife is best known for its frequently occurring replicators.^[\[52\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-54)^^[\[53\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-55)^

Additional Life-like cellular automata exist. The vast majority of these 2^18^ different rules^[\[54\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-56)^ produce universes that are either too chaotic or too desolate to be of interest, but a large subset do display interesting behaviour. A further generalization produces the *isotropic* rulespace, with 2^102^ possible cellular automaton rules^[\[55\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-57)^ (the Game of Life again being one of them). These are rules that use the same square grid as the Life-like rules and the same eight-cell neighbourhood, and are likewise invariant under rotation and reflection. However, in isotropic rules, the positions of neighbour cells relative to each other may be taken into account in determining a cell's future state---not just the total number of those neighbours.

[![](https://upload.wikimedia.org/wikipedia/commons/8/86/Oscillator.gif?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail_unscaled)](https://en.wikipedia.org/wiki/File:Oscillator.gif)

A sample of a 48-step oscillator along with 2-step and 4-step oscillators from a two-dimensional hexagonal Game of Life (rule H:B2/S34)

Some variations on The Game of Life modify the geometry of the universe as well as the rules. The above variations can be thought of as a two-dimensional square, because the world is two-dimensional and laid out in a square grid. One-dimensional square variations, known as [elementary cellular automata](https://en.wikipedia.org/wiki/Elementary_cellular_automaton "Elementary cellular automaton"),^[\[56\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-58)^ and three-dimensional square variations have been developed, as have two-dimensional [hexagonal and triangular](https://en.wikipedia.org/wiki/Regular_tiling "Regular tiling") variations. A variant using [aperiodic tiling](https://en.wikipedia.org/wiki/Aperiodic_tiling "Aperiodic tiling") grids has also been made.^[\[57\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-59)^

Conway's rules may also be generalized such that instead of two states, *live* and *dead*, there are three or more. State transitions are then determined either by a weighting system or by a table specifying separate transition rules for each state; for example, Mirek's Cellebration's multi-coloured Rules Table and Weighted Life rule families each include sample rules equivalent to the Game of Life.

Patterns relating to fractals and fractal systems may also be observed in certain Life-like variations. For example, the automaton B1/S12 generates four very close approximations to the [Sierpinski triangle](https://en.wikipedia.org/wiki/Sierpinski_triangle "Sierpinski triangle") when applied to a single live cell. The Sierpinski triangle can also be observed in the Game of Life by examining the long-term growth of an infinitely long single-cell-thick line of live cells,^[\[58\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-60)^ as well as in Highlife, [Seeds (B2/S)](https://en.wikipedia.org/wiki/Seeds_(cellular_automaton) "Seeds (cellular automaton)"), and [Stephen Wolfram](https://en.wikipedia.org/wiki/Stephen_Wolfram "Stephen Wolfram")'s [Rule 90](https://en.wikipedia.org/wiki/Rule_90 "Rule 90").^[\[59\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-61)^

Immigration is a variation that is very similar to the Game of Life, except that there are two *on* states, often expressed as two different colours. Whenever a new cell is born, it takes on the on state that is the majority in the three cells that gave it birth. This feature can be used to examine interactions between [spaceships](https://en.wikipedia.org/wiki/Spaceship_(cellular_automaton) "Spaceship (cellular automaton)") and other objects within the game.^[\[60\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-62)^ Another similar variation, called QuadLife, involves four different on states. When a new cell is born from three different on neighbours, it takes the fourth value, and otherwise, like Immigration, it takes the majority value.^[\[61\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-63)^ Except for the variation among on cells, both of these variations act identically to the Game of Life.

In popular culture
------------------

[Dr. Blob's Organism](https://en.wikipedia.org/wiki/Dr._Blob's_Organism "Dr. Blob's Organism") is a [Shoot 'em up](https://en.wikipedia.org/wiki/Shoot_'em_up "Shoot 'em up") based on Conway's Life. In the game, Life continually generates on a group of cells within a "[petri dish](https://en.wikipedia.org/wiki/Petri_dish "Petri dish")". The patterns formed are smoothed and rounded to look like a growing [amoeba](https://en.wikipedia.org/wiki/Amoeba_(genus) "Amoeba (genus)") spewing smaller ones (actually gliders). Special "probes" zap the "blob" to keep it from overflowing the dish while destroying its nucleus.^[\[62\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-64)^

Google implemented an [easter egg](https://en.wikipedia.org/wiki/Easter_egg_(media) "Easter egg (media)") of the Game of Life in 2012. Users who search for the term are shown an implementation of the game in the search results page.^[\[63\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-65)^

Various musical composition techniques use the Game of Life, especially in [MIDI](https://en.wikipedia.org/wiki/MIDI "MIDI") sequencing.^[\[64\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-66)^ A variety of programs exist for creating sound from patterns generated in the Game of Life.^[\[65\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-67)^^[\[66\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-68)^^[\[67\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-69)^

See also
--------

-   [Artificial life](https://en.wikipedia.org/wiki/Artificial_life "Artificial life") -- Field of study
-   [*Glory Season*](https://en.wikipedia.org/wiki/Glory_Season "Glory Season") -- 1993 science fiction novel by David Brin, is set in a future society where the Game of Life is played in a competitive two-player mode
-   [Langton's ant](https://en.wikipedia.org/wiki/Langton's_ant "Langton's ant") -- Two-dimensional Turing machine with emergent behavior
-   [Poietic Generator](https://en.wikipedia.org/wiki/Poietic_Generator "Poietic Generator") -- Social network game played on a two-dimensional matrix, a "human" Game of Life.
-   [Self-organization § Computer science](https://en.wikipedia.org/wiki/Self-organization#Computer_science "Self-organization")
-   [*Of Man and Manta*](https://en.wikipedia.org/wiki/Of_Man_and_Manta "Of Man and Manta") -- Trilogy of science fiction novels by Piers Anthony; the novel 'OX' features a cellular automaton lifeform based on Game of Life
-   [LifeWiki](https://en.wikipedia.org/wiki/LifeWiki "LifeWiki") -- Wiki dedicated to Conway's Game of Life
-   [Boids](https://en.wikipedia.org/wiki/Boids "Boids") -- Artificial life algorithm (simulation of [flocking](https://en.wikipedia.org/wiki/Flocking "Flocking") birds)

Notes
-----

1.  *B* for *birth* and *S* for *survive*
2.  The simultaneity means that when each cell counts the number of live neighbours around it, it uses its neighbours' old states before the update, not their new states after the update. If the cells are instead updated in reading order, so that each cell uses the old states of the cells to its right and below it but the new states of the cells to its left and above it, a different cellular automaton results, which is known as NaiveLife^[\[5\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-6)^^[\[6\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-7)^ because it is a common beginners' mistake among people attempting to program Conway's Game of Life.^[\[7\]](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#cite_note-8)^

References
----------

1.  *[Gardner, Martin](https://en.wikipedia.org/wiki/Martin_Gardner "Martin Gardner") (October 1970). ["The fantastic combinations of John Conway's new solitaire game 'life'"](https://web.stanford.edu/class/sts145/Library/life.pdf) (PDF). Mathematical Games. *[Scientific American](https://en.wikipedia.org/wiki/Scientific_American "Scientific American")*. Vol. 223, no. 4\. pp. 120--123\. [doi](https://en.wikipedia.org/wiki/Doi_(identifier) "Doi (identifier)"):[10.1038/scientificamerican1070-120](https://doi.org/10.1038%2Fscientificamerican1070-120). [JSTOR](https://en.wikipedia.org/wiki/JSTOR_(identifier) "JSTOR (identifier)") [24927642](https://www.jstor.org/stable/24927642). [Archived](https://ghostarchive.org/archive/20221009/https://web.stanford.edu/class/sts145/Library/life.pdf) (PDF) from the original on 2022-10-09.*
2.  *[Berlekamp, E. R.](https://en.wikipedia.org/wiki/Elwyn_Berlekamp "Elwyn Berlekamp"); [Conway, John Horton](https://en.wikipedia.org/wiki/John_Horton_Conway "John Horton Conway"); [Guy, R. K.](https://en.wikipedia.org/wiki/Richard_K._Guy "Richard K. Guy") (2001--2004). [*Winning Ways for your Mathematical Plays*](https://en.wikipedia.org/wiki/Winning_Ways_for_your_Mathematical_Plays "Winning Ways for your Mathematical Plays") (2nd ed.). A K Peters Ltd.*
3.  *Roberts, Siobhan (28 December 2020). ["The Lasting Lessons of John Conway's Game of Life"](https://www.nytimes.com/2020/12/28/science/math-conway-game-of-life.html). *[The New York Times](https://en.wikipedia.org/wiki/The_New_York_Times "The New York Times")*.*
4.  *Johnston, Nathaniel; Greene, Dave (2022). [*Conway's Game of Life Mathematics and Construction*](https://conwaylife.com/book/conway_life_book.pdf) (PDF). Retrieved 30 May 2026.*
5.  *["NaiveLife Emulated: A reading-order simulation of Life"](https://conwaylife.com/forums/viewtopic.php?f=11&t=4523&p=128919). *ConwayLife.com*. 24 May 2020. Retrieved 29 November 2021.*
6.  *Goucher, Adam. ["Re: Thread For Your Accidental Discoveries"](https://conwaylife.com/forums/viewtopic.php?f=2&t=279&p=13168#p13168). *ConwayLife.com*. [Archived](https://web.archive.org/web/20211129154301/https://www.conwaylife.com/forums/viewtopic.php?f=2&t=279&p=13168#p13168) from the original on 29 November 2021. Retrieved 29 November 2021.*
7.  *Ian07. ["Re: Strange spaceship that is supposed to be impossible and infinite cell spread"](https://web.archive.org/web/20211129164454/https://conwaylife.com/forums/viewtopic.php?f=9&t=4288&p=88701#p88701). *ConwayLife.com*. Archived from [the original](https://conwaylife.com/forums/viewtopic.php?f=9&t=4288&p=88701#p88701) on 29 November 2021. Retrieved 29 November 2021. *I'm pretty sure this is because you've accidentally created an implementation of what's sometimes known as NaiveLife (as it's a common mistake made by many people coding CGoL for the first time):**
8.  Conway, private communication to the 'Life list', 14 April 1999.
9.  It is a model and simulation that is interesting to watch and can show that simple things can become complicated problems.*Paul Chapman (11 November 2002). ["Life Universal Computer"](https://web.archive.org/web/20090906014935/http://www.igblan.free-online.co.uk/igblan/ca/). Archived from [the original](http://www.igblan.free-online.co.uk/igblan/ca/) on 6 September 2009. Retrieved 12 July 2009.*
10.  *Paul Rendell (January 12, 2005). ["A Turing Machine in Conway's Game of Life"](http://rendell-attic.org/gol/tm.htm). [Archived](https://web.archive.org/web/20190417075720/http://rendell-attic.org/gol/tm.htm) from the original on April 17, 2019. Retrieved July 12, 2009.*
11.  *Stephen A. Silver. ["Glider"](https://conwaylife.com/ref/lexicon/lex_g.htm#glider). The Life Lexicon. Retrieved March 4, 2019.*
12.  *["Census Results in Conway's Game of Life"](https://web.archive.org/web/20090910010855/https://conwaylife.com/soup/census.asp?rule=B3%2FS23&sl=1&os=1&ss=1). The Online Life-Like CA Soup Search. Archived from [the original](https://conwaylife.com/soup/census.asp?rule=B3/S23&sl=1&os=1&ss=1) on 2009-09-10. Retrieved July 12, 2009.*
13.  *["Spontaneous appeared Spaceships out of Random Dust"](http://wwwhomes.uni-bielefeld.de/achim/moving.html). Achim Flammenkamp (1995-12-09). [Archived](https://web.archive.org/web/20090413192821/http://wwwhomes.uni-bielefeld.de/achim/moving.html) from the original on 2009-04-13. Retrieved July 10, 2012.*
14.  *Stephen A. Silver. ["Pulsar"](https://conwaylife.com/ref/lexicon/lex_p.htm#pulsar). The Life Lexicon. Retrieved March 4, 2019.*
15.  *["Census"](https://catagolue.hatsya.com/census/b3s23/C1/xp3). *Catagolue*. Retrieved 30 May 2026.*
16.  *Brown, Nico; Cheng, Carson; Jacobi, Tanner; Karpovich, Maia; Merzenich, Matthias; Raucci, David; Riley, Mitchell (5 December 2023). "Conway's Game of Life is Omniperiodic". [arXiv](https://en.wikipedia.org/wiki/ArXiv_(identifier) "ArXiv (identifier)"):[2312.02799](https://arxiv.org/abs/2312.02799) \[[math.CO](https://arxiv.org/archive/math.CO)\].*
17.  *Stone, Alex (2024-01-18). ["Math's 'Game of Life' Reveals Long-Sought Repeating Patterns"](https://www.quantamagazine.org/maths-game-of-life-reveals-long-sought-repeating-patterns-20240118/). *Quanta Magazine*. [Archived](https://web.archive.org/web/20240118161936/https://www.quantamagazine.org/maths-game-of-life-reveals-long-sought-repeating-patterns-20240118/) from the original on 2024-01-18. Retrieved 2024-01-18.*
18.  *Achim Flammenkamp (2004-09-07). ["Most seen natural occurring ash objects in Game of Life"](http://wwwhomes.uni-bielefeld.de/achim/freq_top_life.html). [Archived](https://web.archive.org/web/20081022033319/http://wwwhomes.uni-bielefeld.de/achim/freq_top_life.html) from the original on 2008-10-22. Retrieved 2008-09-16.*
19.  *Stephen A. Silver. ["Diehard"](https://conwaylife.com/ref/lexicon/lex_d.htm#diehard). The Life Lexicon. Retrieved March 4, 2019.*
20.  *Koenig, H. (February 21, 2005). ["New Methuselah Records"](https://web.archive.org/web/20190910130327/http://pentadecathlon.com/lifeNews/2005/02/new_methuselah_records.html). Archived from [the original](http://pentadecathlon.com/lifeNews/2005/02/new_methuselah_records.html) on September 10, 2019. Retrieved January 24, 2009.*
21.  *Stephen A. Silver. ["Gosper glider gun"](https://conwaylife.com/ref/lexicon/lex_g.htm#gosperglidergun). The Life Lexicon. Retrieved March 4, 2019.*
22.  [The Hunting of the New Herschel Conduits](https://conwaylife.com/forums/viewtopic.php?f=2&t=1599&start=200#p19125) [Archived](https://web.archive.org/web/20220224001858/https://conwaylife.com/forums/viewtopic.php?f=2&t=1599&start=200#p19125) 2022-02-24 at the [Wayback Machine](https://en.wikipedia.org/wiki/Wayback_Machine "Wayback Machine"), ConwayLife forums, April 28th, 2015, posts by [Michael Simkin](https://en.wikipedia.org/wiki/Michael_Simkin?action=edit&redlink=1 "Michael Simkin (page does not exist)") ("simsim314") and Dongook Lee ("Scorbie").
23.  *Stephen A. Silver. ["Infinite Growth"](https://conwaylife.com/ref/lexicon/lex_i.htm#infinitegrowth). The Life Lexicon. Retrieved March 4, 2019.*
24.  *Stephen A. Silver. ["Rake"](https://conwaylife.com/ref/lexicon/lex_r.htm#rake). The Life Lexicon. [Archived](https://web.archive.org/web/20190301232016/http://www.conwaylife.com/ref/lexicon/lex_r.htm#rake) from the original on March 1, 2019. Retrieved March 4, 2019.*
25.  *Hensel, Alan. ["Conway's Game of Life"](https://www.ibiblio.org/lifepatterns/lifep.zip). *[ibiblio](https://en.wikipedia.org/wiki/Ibiblio "Ibiblio")*. Retrieved 30 May 2026.*
26.  *["Programmable computer"](https://conwaylife.com/forums/viewtopic.php?f=2&t=2561#p37428). conwaylife.com forums. Retrieved August 23, 2018.*
27.  *["A Turing Machine in Conway's Game of Life, extendable to a Universal Turing Machine"](http://rendell-attic.org/gol/tm.htm). Paul Rendell. [Archived](https://web.archive.org/web/20190417075720/http://rendell-attic.org/gol/tm.htm) from the original on April 17, 2019. Retrieved August 23, 2018.*
28.  *["Build a working game of Tetris in Conway's Game of Life"](https://codegolf.stackexchange.com/questions/11880/build-a-working-game-of-tetris-in-conways-game-of-life/142673#142673). StackExchange. Retrieved August 23, 2018.*
29.  *Aron, Jacob (16 June 2010). ["First replicating creature spawned in life simulator"](https://www.newscientist.com/article/mg20627653.800-first-replicating-creature-spawned-in-life-simulator.html). *New Scientist*. Retrieved 12 October 2013.*
30.  *["Universal Constructor Based Spaceship"](https://conwaylife.com/forums/viewtopic.php?f=2&t=399&p=2327#p2327). Conwaylife.com. Retrieved 2012-06-24.*
31.  *["Demonoid"](https://conwaylife.com/wiki/Demonoid). LifeWiki. Retrieved 18 June 2016.*
32.  *["Elementary knightship"](https://conwaylife.com/forums/viewtopic.php?f=2&t=3303). Retrieved 9 March 2018.*
33.  ["Elementary"](https://conwaylife.com/wiki/Elementary), LifeWiki. Retrieved 2018-11-21
34.  *["Geminoid Challenge"](https://conwaylife.com/forums/viewtopic.php?f=2&t=1006&p=9917#p9901). Conwaylife.com. Retrieved 2015-06-25.*
35.  *Passe-Science (2019-05-29). ["Automate Cellulaire - Passe-science #27"](https://www.youtube.com/watch?v=CfRSVPhzN5M). [Archived](https://ghostarchive.org/varchive/youtube/20211211/CfRSVPhzN5M) from the original on 2021-12-11. Retrieved 2019-06-25 -- via [YouTube](https://en.wikipedia.org/wiki/YouTube "YouTube").*
36.  *apgoucher (2018-11-12). ["Fully self-directed replication"](https://cp4space.wordpress.com/2018/11/12/fully-self-directed-replication/). *Complex Projective 4-Space*. Retrieved 2019-06-25.*
37.  *["0E0P metacell - LifeWiki"](https://conwaylife.com/wiki/0E0P). *conwaylife.com*. Retrieved 2019-06-24.*
38.  *Alstrøm, Preben; Leão, João (1994-04-01). ["Self-organized criticality in the *game of Life*"](https://link.aps.org/doi/10.1103/PhysRevE.49.R2507). *[Physical Review E](https://en.wikipedia.org/wiki/Physical_Review_E "Physical Review E")*. **49** (4): R2507--R2508. [Bibcode](https://en.wikipedia.org/wiki/Bibcode_(identifier) "Bibcode (identifier)"):[1994PhRvE..49.2507A](https://ui.adsabs.harvard.edu/abs/1994PhRvE..49.2507A). [doi](https://en.wikipedia.org/wiki/Doi_(identifier) "Doi (identifier)"):[10.1103/PhysRevE.49.R2507](https://doi.org/10.1103%2FPhysRevE.49.R2507). [PMID](https://en.wikipedia.org/wiki/PMID_(identifier) "PMID (identifier)") [9961636](https://pubmed.ncbi.nlm.nih.gov/9961636).*
39.  *Dennett, D. C. (1991). [*Consciousness Explained*](https://archive.org/details/consciousnessexp00denn). Boston: Back Bay Books. [ISBN](https://en.wikipedia.org/wiki/ISBN_(identifier) "ISBN (identifier)") [978-0-316-18066-5](https://en.wikipedia.org/wiki/Special:BookSources/978-0-316-18066-5 "Special:BookSources/978-0-316-18066-5").*
40.  *Dennett, D.C. (1995). [*Darwin's Dangerous Idea: Evolution and the Meanings of Life*](https://archive.org/details/darwinsdangerous0000denn). New York: Simon & Schuster. [ISBN](https://en.wikipedia.org/wiki/ISBN_(identifier) "ISBN (identifier)") [978-0-684-82471-0](https://en.wikipedia.org/wiki/Special:BookSources/978-0-684-82471-0 "Special:BookSources/978-0-684-82471-0").*
41.  *Dennett, D.C. (2003). *Freedom Evolves*. New York: Penguin Books. [ISBN](https://en.wikipedia.org/wiki/ISBN_(identifier) "ISBN (identifier)") [978-0-14-200384-8](https://en.wikipedia.org/wiki/Special:BookSources/978-0-14-200384-8 "Special:BookSources/978-0-14-200384-8").*
42.  *Andrzej Okrasinski. ["Game of Life Object Statistics"](https://web.archive.org/web/20090727010353/http://geocities.com/conwaylife/). Archived from [the original](http://www.geocities.com/conwaylife/) on 2009-07-27. Retrieved July 12, 2009.*
43.  *Nathaniel Johnston. ["The Online Life-Like CA Soup Search"](https://web.archive.org/web/20090910010849/https://conwaylife.com/soup/). Archived from [the original](https://conwaylife.com/soup/) on 2009-09-10. Retrieved July 12, 2009.*
44.  *Helmers, Carl (June 1976). ["About the Cover"](https://archive.org/stream/byte-magazine-1976-06/1976_06_BYTE_00-10_The_Game_of_LIFE_in_Color#page/n7/mode/2up). *[Byte](https://en.wikipedia.org/wiki/Byte_(magazine) "Byte (magazine)")*. No. 10\. pp. 6--7. Retrieved February 18, 2013.*
45.  *[McIntosh, Harold](https://en.wikipedia.org/wiki/Harold_V._McIntosh "Harold V. McIntosh") (2008). ["Introduction"](http://comunidad.escom.ipn.mx/LCCOMP/Announces/Entries/2015/12/23_LCCOMP_Obituaries_2015_files/181-186pp%20JCA-HM07-00.pdf) (PDF). *Journal of Cellular Automata*. **13**: 181--186\. [Archived](https://ghostarchive.org/archive/20221009/http://comunidad.escom.ipn.mx/LCCOMP/Announces/Entries/2015/12/23_LCCOMP_Obituaries_2015_files/181-186pp%20JCA-HM07-00.pdf) (PDF) from the original on 2022-10-09. Retrieved 3 November 2021. With the advent of microcomputers and Cromemco's graphics board, Life became a favorite display program for video monitors and led to a revival of interest in the game.*
46.  *["Acorn User Magazine Scans"](http://8bs.com/aumags.htm). The BBC and Master Computer Public Domain Library. Retrieved 2018-12-29.*
47.  *Stepney, Susan. ["AcornUser articles"](https://www-users.cs.york.ac.uk/susan/bib/ss/au.htm). *www-users.cs.york.ac.uk*. AcornUser. Retrieved 2018-12-29.*
48.  *Alan Hensel. ["About my Conway's Game of Life Applet"](http://www.ibiblio.org/lifepatterns/lifeapplet.html). [Archived](https://web.archive.org/web/20090716231902/http://www.ibiblio.org/lifepatterns/lifeapplet.html) from the original on July 16, 2009. Retrieved July 12, 2009.*
49.  *Nehaniv, Chrystopher L. (15--18 July 2002). *Self-Reproduction in Asynchronous Cellular Automata*. [2002 NASA/DoD Conference on Evolvable Hardware](https://ieeexplore.ieee.org/xpl/conhome/8000/proceeding). Alexandria, Virginia, USA: IEEE Computer Society Press. pp. 201--209\. [doi](https://en.wikipedia.org/wiki/Doi_(identifier) "Doi (identifier)"):[10.1109/EH.2002.1029886](https://doi.org/10.1109%2FEH.2002.1029886). [hdl](https://en.wikipedia.org/wiki/Hdl_(identifier) "Hdl (identifier)"):[2299/6834](https://hdl.handle.net/2299%2F6834). [ISBN](https://en.wikipedia.org/wiki/ISBN_(identifier) "ISBN (identifier)") [0-7695-1718-8](https://en.wikipedia.org/wiki/Special:BookSources/0-7695-1718-8 "Special:BookSources/0-7695-1718-8").*
50.  *["Conway's Game of Life"](https://rosettacode.org/wiki/Conway%27s_Game_of_Life). *Rosetta Code*. June 7, 2024. [Archived](https://web.archive.org/web/20240718213019/https://rosettacode.org/wiki/Conway%27s_Game_of_life) from the original on July 18, 2024. Retrieved July 2, 2024.*
51.  *["Xlife - LifeWiki"](https://conwaylife.com/wiki/Xlife). *conwaylife.com*.*
52.  [HighLife -- An Interesting Variant of Life](http://www.tip.net.au/~dbell/articles/HighLife.zip) by David Bell (.zip file)
53.  *Stephen A. Silver. ["Replicator"](https://conwaylife.com/ref/lexicon/lex_r.htm#replicator). The Life Lexicon. [Archived](https://web.archive.org/web/20190301232016/http://www.conwaylife.com/ref/lexicon/lex_r.htm#replicator) from the original on March 1, 2019. Retrieved March 4, 2019.*
54.  *["Life-like cellular automata - LifeWiki"](https://conwaylife.com/wiki/Life-like#Life-like_cellular_automata). Conwaylife.com. [Archived](https://web.archive.org/web/20190306043758/http://conwaylife.com/wiki/Life-like#Life-like_cellular_automata) from the original on March 6, 2019. Retrieved March 4, 2019.*
55.  *["Isotropic - LifeWiki"](https://conwaylife.com/wiki/Isotropic). Conwaylife.com. [Archived](https://web.archive.org/web/20190306043649/http://conwaylife.com/wiki/Isotropic) from the original on March 6, 2019. Retrieved March 4, 2019.*
56.  *["Elementary Cellular Automaton"](http://mathworld.wolfram.com/ElementaryCellularAutomaton.html). Wolfram Mathworld. [Archived](https://web.archive.org/web/20090703200815/http://mathworld.wolfram.com/ElementaryCellularAutomaton.html) from the original on July 3, 2009. Retrieved July 12, 2009.*
57.  *["First gliders navigate ever-changing Penrose universe"](https://www.newscientist.com/article/dn22134-first-gliders-navigate-everchanging-penrose-universe.html). *New Scientist*.*
58.  [Stephen Wolfram](https://en.wikipedia.org/wiki/Stephen_Wolfram "Stephen Wolfram"), *[A New Kind of Science](https://en.wikipedia.org/wiki/A_New_Kind_of_Science "A New Kind of Science")* online, [Note (f) for structures in class 4 systems: Structures in the Game of Life](https://www.wolframscience.com/nks/notes-6-8--structures-in-the-game-of-life/): "A simpler kind of unbounded growth occurs if one starts from an infinite line of black cells. In that case, the evolution is effectively 1D, and turns out to follow elementary rule 22"
59.  *["Life Imitates Sierpinski"](https://conwaylife.com/forums/viewtopic.php?f=7&t=90). ConwayLife.com forums. Retrieved July 12, 2009.*
60.  *Stephen A. Silver. ["Immigration"](https://conwaylife.com/ref/lexicon/lex_i.htm#immigration). The Life Lexicon. Retrieved March 4, 2019.*
61.  *Stephen A. Silver. ["QuadLife"](https://conwaylife.com/ref/lexicon/lex_q.htm#quadlife). The Life Lexicon. Retrieved March 4, 2019.*
62.  *Cohen, Peter (23 September 2003). ["Dr. Blob's Organism oozes onto Mac OS X"](https://www.macworld.com/article/168098/organism.html). *[Macworld](https://en.wikipedia.org/wiki/Macworld "Macworld")*. [International Data Group](https://en.wikipedia.org/wiki/International_Data_Group "International Data Group"). [Archived](https://web.archive.org/web/20210515185231/https://www.macworld.com/article/168098/organism.html) from the original on 15 May 2021. Retrieved 8 July 2025.*
63.  *Wasserman, Todd (12 July 2012). ["Type 'Conway's Game of Life' on Google and See What Happens"](https://mashable.com/2012/07/12/conways-game-of-life-google/). *[Mashable](https://en.wikipedia.org/wiki/Mashable "Mashable")*. Retrieved 1 May 2020.*
64.  *Burraston, Dave; Edmonds, Ernest; Livingstone, Dan; [Miranda, Eduardo Reck](https://en.wikipedia.org/wiki/Eduardo_Reck_Miranda "Eduardo Reck Miranda") (2004). ["Cellular Automata in MIDI based Computer Music"](http://quod.lib.umich.edu/i/icmc/bbp2372.2004.047?view=image). *Proceedings of the 2004 International Computer Music Conference*. [hdl](https://en.wikipedia.org/wiki/Hdl_(identifier) "Hdl (identifier)"):[10453/1425](https://hdl.handle.net/10453%2F1425). [ISBN](https://en.wikipedia.org/wiki/ISBN_(identifier) "ISBN (identifier)") [978-0-9713192-2-6](https://en.wikipedia.org/wiki/Special:BookSources/978-0-9713192-2-6 "Special:BookSources/978-0-9713192-2-6"). [Archived](https://web.archive.org/web/20230111115704/https://quod.lib.umich.edu/i/icmc/bbp2372.2004.047?view=image) from the original on 2023-01-11. Retrieved 2012-07-05.*
65.  *["glitchDS -- Cellular Automaton Sequencer For The Nintendo DS"](http://www.synthtopia.com/content/2008/05/29/glitchds-cellular-automaton-sequencer-for-the-nintendo-ds/). Synthtopia.com. 2008-05-29. [Archived](https://web.archive.org/web/20120726170457/http://www.synthtopia.com/content/2008/05/29/glitchds-cellular-automaton-sequencer-for-the-nintendo-ds/) from the original on 2012-07-26. Retrieved 2012-06-24.*
66.  *["Game Of Life Music Sequencer"](http://www.synthtopia.com/content/2009/04/29/game-of-life-music-sequencer/). Synthtopia.com. 2009-04-29. [Archived](https://web.archive.org/web/20120726113942/http://www.synthtopia.com/content/2009/04/29/game-of-life-music-sequencer/) from the original on 2012-07-26. Retrieved 2012-06-24.*
67.  *["Game Of Life Music Sequencer For iOS, Runxt Life"](http://www.synthtopia.com/content/2011/01/12/game-of-life-music-sequencer-for-ios-runxt-life/). Synthtopia.com. 2011-01-12. [Archived](https://web.archive.org/web/20120726014246/http://www.synthtopia.com/content/2011/01/12/game-of-life-music-sequencer-for-ios-runxt-life/) from the original on 2012-07-26. Retrieved 2012-06-24.*

External links
--------------

[![Wikimedia Commons logo](https://thumb.wikimedia.org/wikipedia/en/thumb/4/4a/Commons-logo.svg/40px-Commons-logo.svg.png?utm_source=en.wikipedia.org&utm_campaign=parser&utm_content=thumbnail)](https://en.wikipedia.org/wiki/File:Commons-logo.svg)
Wikimedia Commons has media related to [Game of Life](https://commons.wikimedia.org/wiki/Game%20of%20Life "commons:Game of Life").

-   [Life Lexicon](https://conwaylife.com/ref/lexicon/lex_home.htm) *conwaylife.com:* extensive lexicon with many patterns
-   [LifeWiki](https://conwaylife.com/wiki/) *conwaylife.com*
-   [ConwayLife.com Forums](https://conwaylife.com/forums/) conwaylife.com
-   [Catagolue](https://catagolue.hatsya.com/home) *cata*gol*ue.hatsya.com*: online database of objects in Conway's Game of Life and similar cellular automata
-   [Cellular Automata FAQ -- Conway's Game of Life](https://cafaq.com/lifefaq/index.php) *cafaq.com*
-   [Algebraic formula](https://uk.mathworks.com/matlabcentral/fileexchange/80176-conways-game-of-life-equation) *uk.mathworks.com*: [recurrence relation](https://en.wikipedia.org/wiki/Recurrence_relation "Recurrence relation") for iterating Conway's Game of Life.

| -   [v](https://en.wikipedia.org/wiki/Template:Conway's_Game_of_Life "Template:Conway's Game of Life")-   [t](https://en.wikipedia.org/wiki/Template_talk:Conway's_Game_of_Life "Template talk:Conway's Game of Life")-   [e](https://en.wikipedia.org/wiki/Special:EditPage/Template:Conway's_Game_of_Life "Special:EditPage/Template:Conway's Game of Life")[Conway's Game of Life](https://en.wikipedia.org/wiki/Conway's_Game_of_Life) and related [cellular automata](https://en.wikipedia.org/wiki/Cellular_automaton "Cellular automaton") ||
| --- | --- |
|  | -   -   -   -   -   -   -   -   -   -   -   -   -   -   -    |
|  | -   -   -   -   -    |
|  | -   -   -   -    |
|  | -   -   **-   **-   ** |
|  | -   -   -   -    |
|  | -    |
|  | -   **-   ** |

[Categories](https://en.wikipedia.org/wiki/Help:Category "Help:Category"):

-   [Cellular automaton rules](https://en.wikipedia.org/wiki/Category:Cellular_automaton_rules "Category:Cellular automaton rules")
-   [Self-organization](https://en.wikipedia.org/wiki/Category:Self-organization "Category:Self-organization")
-   [Games and sports introduced in 1970](https://en.wikipedia.org/wiki/Category:Games_and_sports_introduced_in_1970 "Category:Games and sports introduced in 1970")
-   [John Horton Conway](https://en.wikipedia.org/wiki/Category:John_Horton_Conway "Category:John Horton Conway")

-   This page was last edited on 9 September 2026, at 16:35 (UTC).
-   Page was rendered with [Parsoid](https://www.mediawiki.org/wiki/Special:MyLanguage/Parsoid "mw:Special:MyLanguage/Parsoid").
-   Text is available under the [Creative Commons Attribution-ShareAlike 4.0 License](https://en.wikipedia.org/wiki/Wikipedia:Text_of_the_Creative_Commons_Attribution-ShareAlike_4.0_International_License "Wikipedia:Text of the Creative Commons Attribution-ShareAlike 4.0 International License"); additional terms may apply. By using this site, you agree to the [Terms of Use](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Policy:Terms_of_Use "foundation:Special:MyLanguage/Policy:Terms of Use") and [Privacy Policy](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Policy:Privacy_policy "foundation:Special:MyLanguage/Policy:Privacy policy"). Wikipedia® is a registered trademark of the [Wikimedia Foundation, Inc.](https://wikimediafoundation.org/), a non-profit organization.

-   [Privacy policy](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Policy:Privacy_policy)
-   [About Wikipedia](https://en.wikipedia.org/wiki/Wikipedia:About)
-   [Disclaimers](https://en.wikipedia.org/wiki/Wikipedia:General_disclaimer)
-   [Contact Wikipedia](https://en.wikipedia.org/wiki/Wikipedia:Contact_us)
-   [Legal & safety contacts](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Legal:Wikimedia_Foundation_Legal_and_Safety_Contact_Information)
-   [Code of Conduct](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Policy:Universal_Code_of_Conduct)
-   [Developers](https://developer.wikimedia.org/)
-   [Statistics](https://stats.wikimedia.org/#/en.wikipedia.org)
-   [Cookie statement](https://foundation.wikimedia.org/wiki/Special:MyLanguage/Policy:Cookie_statement)
-   [Mobile view](https://en.wikipedia.org/w/index.php?title=Conway%27s_Game_of_Life&mobileaction=toggle_view_mobile)

-   [![Wikimedia Foundation](https://en.wikipedia.org/static/images/footer/wikimedia.svg)](https://www.wikimedia.org/)
-   [![Powered by MediaWiki](https://en.wikipedia.org/w/resources/assets/mediawiki_compact.svg)](https://www.mediawiki.org/)