# Ace_Pokedex

Project Proposal
Our project is a Pokemon social media platform built as a full-stack web application. The goal of the project was to create a community driven tool where users can explore Pokemon data, build teams, and interacts with other players through posts, and comments.

The application pulls live Pokemon data from the public PokeAPI and presents it in a clean, browsable interface. Registered users can create and manage teams with up to six Pokemon, assigning each member a moveset, ability, nature, and held item. Users can also write posts, react to other users' post with likes or dislikes, and leave comments.

Features include.
* Browser Pokemon with stats, types, abilites, and moves
* View a full Type Chart showing type matchups
* Browser Held Items, Moves, Abilities, and Natures
* User register and login with secure authentication
* Team builder with full customization per Pokemon slot
* Comunity posts with reactions and comments
* User profiles showing teams and activities

A.I Disclosure

Which A.I. tools did you use?
We used Claude by Anthropic throughout the development of this project.

How did you use them?
Claude was used as a reference and guidance tool during development. When we ran into roadblocks like setting up cookie-based authentication, structuring our service layer, or connecting to the PokeAPI, we used Claude as a way to help us understand the concepts and show us ways of doing things that were not by it generating code. It was also used to help us work through JavaScript, mostly around building the interactive page behaviors such as auto-dismissing alerts, delete confirmation dialogs, and live character counters on forms. Claude helped us understand how to manipulate the DOM and structure client-side logic in a clean, organized way.

What specific insights or assistance have you gained?
Through working with Claude, we gained a better understanding of how ASP.NET Core MVC ties together controllers, services, and views. We learned how Entity Framework Core handles database migrations and relationships, how cookie authentications works, and how to properly consume an external REST API using a typed HTTP Client. On the frontend side, we gained a clearer understanding of how JavaScript interacts with the DOM and how to organize client-side code effectively, Claude helped us connect the dots between concepts we had seen in class and how they get applied in a project, while ensuring the work and understanding remained out own.


Accessibilty principles followed
* Semantic HTML - pages use proper HTML elements like <nav>, <header>, <main>, and <footer> so the page strucute is clear.
* Alt text on images - all Pokemon sprites and images include descriptive alt attributes.
* Responsive desing - the layout adapts to all screen sizes including mobile devices.
* Form labels - all form inputs have proper <label> elements so users know what each field is for.

