# Bingo

I recently heard about the Bingo paradox, where you're more likely to get a winning card with a horizontal line than a vertical line. Figured I'd code up a quick test to check it out.

For some background, a Bingo card is a 5x5 card of numbers ranging from 1 to 75. There are 5 columns labeled B, I, N, G, and O. 
- B contains 5 random numbers between 1 and 15
- I contains 5 random numbers between 16 and 30
- N contains 4 random numbers beween 31 and 45. The third row in N is a 'free' space.
- G contains 5 random numbers between 46 and 60
- O contains 5 random numbers between 61 and 75

As the game is played, a random number between 1 and 75 is chosen, and if the card has the matching number, that space on the card is marked. The card is a winner if there is any row, column, or diagonal that is fully marked, meaning every number in that row, column, or diagonal has been marked.

### Take 1
The first way I did it was to just use a 5x5 array. I used a tuple of (int, bool) as the value in the array, with item 1 being the value of the cell, and item 2 being whether the number in that cell has been marked.

This is all in the `Card` class.

> Turns out that, yes, horizontal typically wins about 3 times as much as vertical.

### Take 2
I thought there might be a more efficient way to do it. There's a lot of looping to figure out which cell has the matching number, figuring out if a row, column, or diagonal has been fully selected, etc. The board has to be checked each time a cell is selected, so there's lots of looping.

So I then tried the `Card2` class. It has a couple of dictionaries mapping the (row, col) to an instance of the `Cell` class that contains the value, and another dictionary mapping the value to the `Cell` that contains the value. It also has `CellGroup`s which represents the `Cell` objects in each row, column, and diagonal.

When a value is chosen, it's a quick dictionary lookup to see if the card contains a cell with the chosen value. If it is, the `Cell` then notifies the `CellGroup`s that use the cell, and if a `CellGroup` is fully selected, the card is marked as a winner.

Turns out `Card2` takes much longer to construct, which is acceptable if the runtime performance is faster. The card is only created once, but multiple games are run against it, so runtime performance is more important. Unfortunately, the runtime performance is also much slower than `Card` so it doesn't make sense to use, and I suspect its memory usage is also much greater, so that's a fail.