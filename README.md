# TrackCaloriesBot
_____
Telegram bot for monitoring your food intake and recipe searching.

The bot is written in **C#** using the **Telegram API** and **Webhooks**.
The database is **PostgreSQL** and **Redis** for cache storage.
______
### Functionality

There is a main menu where you can add food intake, add and search for a new recipe, show your progress, show information about you and summary information about today added data.
<p>
<img src="https://user-images.githubusercontent.com/72071649/236805138-6a769849-7996-4784-b0da-d02b09c5781e.jpg" alt="name" width=400>
</p>

_____

#### New record
If you want to make a new record you should specify a food intake or add a water.
<p>
<img src="https://user-images.githubusercontent.com/72071649/236805140-45c624a7-4cdb-4439-a993-f064e201333c.jpg" alt="name" width=400>
</p>

After choosing food intake you can:
- **Search products** - search product from food database
- **Enter manually** - add all food information manually
- **Get from my saved ones** - get a product that you have already saved
<p>
<img src="https://user-images.githubusercontent.com/72071649/236805142-c6bea01c-625e-4dc9-9003-68dace2b7d98.jpg" alt="name" width=400>
</p>

________
#### Recipes   


In **Recipes** you can:
- **Search new recipes** - get a recipe from a database and see a detail information using WebApp
- **Create my own recipe** - write all ingredients and cooking information manually
- **Show my recipes** - get a list of all added recipes by yourself

<p>
<img src="https://user-images.githubusercontent.com/72071649/236805144-f8f4c1a8-6e74-4efa-bb68-3296eb8a9f7b.jpg" alt="name" width=400>
</p><p>
<img src="https://user-images.githubusercontent.com/72071649/236805154-fad9de44-3d3f-4e8a-a639-ad6654420f1d.jpg" alt="name" width=300>
<img src="https://user-images.githubusercontent.com/72071649/236805155-62e18e29-70ad-4441-91ed-3bfedd611a49.jpg" alt="name" width=250>
</p>

_____
#### Summary for today

Shows information about eaten today calories and PFC, eaten calories per each food intake and drank water in liters.
<p>
<img src="https://user-images.githubusercontent.com/72071649/236805151-cfc6e89e-110e-4f28-81fc-58aa1ba05417.jpg" alt="name" width=200>
</p>
