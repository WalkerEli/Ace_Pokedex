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
Claude was used only as a reference and guidance tool during development when we ran into roadblocks like setting up cookie-based authentication, structuring our service layer, and connecting to the PokeAPI. We used Claude to help us understand the concepts and explore approaches to solving problems. Rather than giving us a single answer, Claude would walk us through tradeoffs of each so we could make informed decisions about how to build the project. It was also used to help us work through our JavaScript, particularly around building interactive page behaviors such as auto-dismissing alerts, delete confirmation dialogs, and implementing autocomplete functionality for the team builder using HTML data lists populated with live PokeAPI data.

What specific insights or assistance have you gained?
On of the things we learned about while working with Claude was what DTO (Data Transfer Object) is and why it exists. We learned that a DTO is a simple object whose only job is to match the shape of data coming from an external source. In our case the JSON responses from PokeAPI. Rather than using our application’s own models to catch raw API data, we used DTOs as a middle step, and then our service layer would convert them into View Models shaped the way our views needed the,. This gave us a clearer picture of how data flows through a layered application. Beyond that, we gained a better understanding of how ASP.NET core MVC ties together controllers, services, and views, and how cookie authentication works under the hood, and how to use JavaScript to fetch live data from an API directly in the browser to drive dynamic UI features like autocomplete.



Accessibilty principles followed
* Semantic HTML - pages use proper HTML elements like <nav>, <header>, <main>, and <footer> so the page strucute is clear.
* Alt text on images - all Pokemon sprites and images include descriptive alt attributes.
* Responsive desing - the layout adapts to all screen sizes including mobile devices.
* Form labels - all form inputs have proper <label> elements so users know what each field is for.

