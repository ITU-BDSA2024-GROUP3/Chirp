# **Running Chirp Locally**

## **Cloning the repository**

To run Chirp locally, you must start by cloning the project to your computer. This can be done by going to the program’s GitHub page, clicking the green `<> code` button. Then you can copy the web URL, which would be:

[https://github.com/ITU-BDSA2024-GROUP3/Chirp.git](https://github.com/ITU-BDSA2024-GROUP3/Chirp.git)

You can then either use GitHub Desktop, and click `clone repository`, which can be found under `File`. The link can then be used to clone the repository. Another option is to open a terminal in the folder you wish to clone the repository, then using the following command to clone it:

`git clone https://github.com/ITU-BDSA2024-GROUP3/Chirp.git`

## **Running the program**

To run the program locally on your computer, you will likely need developer tools like dotnet. Once you have the necessary programs installed, you can use this command on a terminal in the root of this repository:

`cd ./src/ChirpWeb`

That will take you to the ChirpWeb folder in the terminal, where the program should be run from. One can then use dotnet to run the program locally, which will start a web server you can access through a link the program will provide. The command to use is as follows:

`dotnet run --launch-profile "https"`

Then one can click on the localhost link, and be taken to the page. The page will start on the public page. Note that should you wish to log in as Helge or Adrian, your username will be the same as your actual name, so Helge's profile has the username Helge, and Adrian's profile has the username Adrian.

Another thing to note is that currently if your username on GitHub is the same as existing user, the program will crash. We know this error will occur, but have been told by Helge that it should be ignored.

# **Test cases**

## **How to run the test cases**

To run test cases, you should return to the root of this repository. There you can use this command to run all tests.

`dotnet test`

Note that if you do not have Playwright installed on your computer, all Playwright tests will fail, but integration and unit tests will still complete.

## **The types of tests we have**

We have 15 unit tests, which are all collected under the `Chirp/test/UnitTest` directory. Unit tests are made to test the functionality of CheepRepository and AuthorRepository. Specifically, there exist tests for most methods in the repositories.

We have 152 integration tests, which are all collected under the `Chirp/test/IntegrationTest` directory. These integration tests focus on testing the resulting web pages by checking for the presence of specific elements.

We have 10 End2End tests, which are all collected under the `Chirp/test/PlaywrightTests` directory. PlaywrightTests test whether the program works as a whole, combining frontend and backend. In other words, it tests the functionality of the deployed web server. In the tests, a test user is created to navigate the web page and use all of its functionalities.
