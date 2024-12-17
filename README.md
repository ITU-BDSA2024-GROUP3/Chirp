# Chirp
Chirp is a twitter clone, which allows users to register themselves, follow, like and 
post cheeps. Cheeps are small messages, tied to a user and a specific timestamp. Chirp 
is hosted on an Azure webpage, until the 21 of january 2025. Here is the link:

https://bdsa2024group03chirprazor.azurewebsites.net/


## Running Chirp Locally
### Cloning the repository
To run Chirp locally, you must start by cloning the project to your computer. This can 
be done by going to the programs github page, clicking the green `<> code` button. 
Then you can copy the web URL, which would be:

https://github.com/ITU-BDSA2024-GROUP3/Chirp.git

You can then either use github desktop, and click `clone repository`, which can be 
found under `File`. Then the link can be used to clone the repository. Another option 
is to open a terminal in the folder you wish to clone the repository, then using the 
following command to clone it:

`git clone https://github.com/ITU-BDSA2024-GROUP3/Chirp.git`

### Running the program
To run the program locally on your computer, you will likely need developer tools like 
dotnet. Once you have the nessecary programs installed, you can use this command on a 
terminal in the root of this repository:

``cd ./src/ChirpWeb``

That will take you to the ChirpWeb folder in the terminal, where the program should be 
run from. One can then use dotnet to run the program locally, which will start a 
webserver you can access through a link the program will provide. The command to use 
is as follows:

``dotnet run --launch-profile "https"``

Then one can click on the localhost link, and be taken to the page. The page will 
start on the public page. Note that should you wish to log in as Helge or Adrian, your 
username will be the same as your actual name, so Helge's profile have the username 
Helge, and Adrian's profile has the username Adrian.

Another thing to note, is that currently if your username on github is the 
same as an already made users username, the program will crash. We know this error 
will occur, but have been told by Helge that it should be ignored.


## Test cases
### How to run the test cases
To run test cases, you should return to the root of this repository. There you can use 
this command to run all tests.

``dotnet test``

Note that if you do not have playwright installed on your computer all playwright 
tests will fail, but integration and unit tests will still complete.

### The types of tests we have
We have 15 unit tests, which are all collected under the ``Chirp/test/UnitTest`` 
directory. These unit tests focus on testing vital repository functions, in a vacuum.

We have 152 integration tests, which are all collected under the 
``Chirp/test/IntegrationTest`` directory. These integration tests focus on testing the 
webpages resulting HTML, whether different pages contain the correct elements.  

We have 10 end2end tests, which are all collected under the 
``Chirp/test/PlaywrightTests`` directory. These tests all use playwright to perform 
tests on the webpages overall functionality.