# Dustcloud.Maze
InfoSys Interview Project

### Foreword
Dear InfoSys people,  
~~<strike>First of all, I'd like to apologize for the fact I have used .NET 6 (and not 5, as specified) - I was a little bit too deep into the project when I realized that little oversight.</strike>~~
(_NB: As of 16/12 7PM the app has now been downgraded to run on .NET 5. If you wish to see code differences between .NET 5 and 6, please check the GIT repository tree_)
  
(Oh, also, please don't mind the 'z's in my verbs, when you hear me you'll know why).  
This has been a really cool project, I must say, this form of interviewing is by far the best. Not only does it showcase my abilities, but also allows me to do some research and teach me stuff along the way
(which I believe is what it's all about in life - constant education).  
It may have taken me a little bit over the **two** hours, that the email said were originally assigned for this project.  
So, without further ado, here it is...

## The application
The application consists of only one main window.  
Most of the visuals are cramped into that MainWindow (as beautifying the UI would most likely take me another day and I'd be glad to, but I'm slowly running out of time--)  
The File menu allows you to load a text file (like ExampleMaze.txt provided by you, or MashMaze.txt provided by me in the solution), or create your own Maze (On the Data Tab), which you can later on save as a text file ;)
  
### Data Tab
To create your own maze just select "New" from the File menu and type (or paste) your maze into the box provided, after which you click on Validate and Parse and (provided your maze did not fail validation) you should see 
your maze on the right hand side of the screen (a crude representation thereof, which I would LOVE to beautify, but as I said, time constraints... however, it should be enough to satisfy the **User story 1 Acceptance Criterion 1 (US1, AC1)**)  
(_NB: For the sake of this exercise, please be advised that at this time only rectangular mazes will pass validation - and please refrain from putting Empty Spaces as the outside tiles--_)

Once loaded (or parsed), the application will tell you how many walls and how many empty spaces there are **(US1, AC2)** and will also allow you to put in Coordinates to check what hides under there **(US1, AC3)**  
(_NB: Same can be accomplished by simply clicking on a tile on the right hand side_)
    
### Manual Tab
Once data is loaded, the Manual tab will allow you to "Drop" an explorer onto the board (the Explorer/Hero always drops onto the Start tile - **(US 2, AC1)**). The Explorer Data groupbox shows you all of the info about your Explorer (Current position and Facing Direction). 
Using the arrow buttons, you can steer your explorer (an arrow) on the board (__NB: You can also use the keyboard cursors__). Every move is logged onto the movement console on the bottom left, as well as showing any possible moves **(UC2, AC2)**.  
Once your explorer reaches the "Exit", the board also lights up the tiles that the explorer has visited (using a blinking Exclamation point) 

### Automatic Tab
_(NB: Now, this was the funnest part, for sure, and took me the longest, which I am not ashamed to admit.)_
Once the date is loaded (or parsed), this tab will allow you to either find all of the possible combinations (routes) to get from Start to Exit (without stepping on the same tile twice), or helps you find the quickest route.
When either of the buttons clicked the data loads (asynchronously (_NB: which can really be only shown on bigger mazes, please use MashMaze.txt - there are 8400 combinations to solve it_)). The loaded data tells you how many moves it took
and clicking on the item (in the upper control) then displays the route history in the bottom control, as well as - you've guessed it, lights it up on the control on the right hand side (using the same exclamation point as before).

## The solution  
The solution consists of four projects 
- Dustcloud.Maze (The App)
- Dustcloud.Maze.Model
- Dustcloud.Maze.Services
- Dustcloud.Maze.Tests

### Dustcloud.Maze (The face of the app)
The first one is where all of the UI stuff lives (my views, my viewmodels, converters, selectors, etc).
At first glance you'll see I have used Prism as my MVVM framework (I usually pick Galasoft's MVVMLight - and actually, 
at the point of choosing the framework I realized I had created the project in .NET 6 and Galasoft had a bit of trouble with it - hence Prism).  
Other dependencies worth mentioning are **ReactiveUI.WPF** and **Dustcloud.IOC.Standard** https://www.nuget.org/packages/Dustcloud.IOC.Universal/  
(_NB: the latter of which is a nuget package of my own creation - a simple wrapper over Microsoft.DependencyInjection - 
the link will actually take you to its sister project that I did for UWP (hence 'Universal'), but there's more documentation on that one, than the standard one ;)_).  
(_NB2: the **D** in SOLID is the coolest part, methinks_).  
  
While I have done some RX in my life in certain businesses (and as hobby projects), this was the first time I've used a package made specifically for WPF. On the one hand, it would have been cool to just use
.ObserveOnDispatcher (which comes out of the box and I did use it at first, while developing), however I quickly realized unit testing that piece of code would be a nightmare, which is why I created the
ISchedulerFactory class to help me mock the schedulers easier (of which later, as it lives in the Services library).  

(_NB: There are a couple of things I do in the code behind of the window, I'll gladly discuss why I chose to do it that way, as in my mind the MVVM pattern is not broken by it_).
  
### Dustcloud.Maze.Model (The heart of the app)
The model is a simple attempt to keep the app an n-tier one.  
Since my Tile class is a base for four other classes (which apart from being derived, don't really do much, except calling on the base with an arbitrary Type enum (which I could easily use to identify the tiles by (and I do in a lot of places))),
and if I had a bit more time I would possibly fix that, but hey, at least it shows off the fact I understand the L part of SOLID ;)  


### Dustcloud.Maze.Services (The brains of the app)
There are really just two services in this project (the **SchedulerFactory** is just a wrapper to allow me to mock things better in the unit tests).
The __DataService__ deals with... data.  
And the FindFinishService (ok, I just realized how dumb a name I gave it, oh well ;)) deals with the manipulation of the explorer and also contains the algorithm to find the routes.  
(_NB: While they actually used to be two classes, while writing this now makes me think it was a good idea to keep them separate - it's not very **S** for SOLID. Another thing to discuss, I hope ;)_).

### Dustcloud.Maze.Tests (The playground!)
I have managed to write tests only for the services, and a complicated one for the MainViewModel. Did I follow TDD to write this app? I'll gladly give you the answer to that at the interview.
  
  
Well, there you have it folks! Hope you've enjoyed my work and I'm looking forward to hearing from you.  
  
Kind regards,  
Greg Chelstowski



