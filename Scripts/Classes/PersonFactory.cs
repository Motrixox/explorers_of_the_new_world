using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Analytics;

public static class PersonFactory
{
	static readonly string[] genders = { "Male", "Female" };
	static readonly string[] maleNames = { "John", "Paul", "Dave", "Oscar", "Silas", "James", "Jack", "William", "Julian", "Ricky" };
	static readonly string[] femaleNames = { "Lucy", "Daisy", "Scarlett", "Jane", "Juliet", "Evelyn", "Lena", "Annie", "Rosie", "Maggie" };
	static readonly string[] foods = { "Fish", "Meat", "Fruit", "Bread", "Vegetable" };
	static readonly string[] goods = { "Cotton", "Jewelry", "Spice", "Wine", "Herb", "Clothes" };
	static readonly string[] buildings = { "Church", "Baths", "Theater", "Pub", "Brothel", "School", "Hospital", "Market" };
	static readonly string[] classes = { "Working class", "Middle class", "Upper class" };
	static readonly string[] wcProfessions = { "Woodcutter", "Hunter", "Mason", "Fisherman", "Collector", "Farmer",
											  "Miner", "Prostitute", "Warehouseman",  "Ship builder",  "Sailor" };
	static readonly string[] mcProfessions = { "Miller", "Baker", "Herbalist", "Salesman", "Guard", "Firefighter", "Waiter", "Bath worker", "Weaver" };
	static readonly string[] ucProfessions = { "Merchant", "Doctor", "Priest", "Actor", "Goldsmith", "Teacher", "Mintsmith" };

	public static Person CreatePerson(int seed)
	{
		var person = new Person();

		person.id = Guid.NewGuid().ToString();

		var random = new System.Random(seed);

		person.gender = genders[random.Next(0, 2)];
		if (person.gender.Equals("Male"))
			person.name = maleNames[random.Next(0, maleNames.Length)];
		else
			person.name = femaleNames[random.Next(0, femaleNames.Length)];
		person.age = random.Next(25, 50);
		person.ageWeeks = random.Next(1, 53);
		person.favFood = foods[random.Next(0, foods.Length)];
		person.favGood = goods[random.Next(0, goods.Length)];
		person.favBuilding = buildings[random.Next(0, buildings.Length)];
		person.morale = 70;
		person.productivity = 30;

		var c = random.Next(1, 101);
		if (c <= 5)
		{
			person._class = classes[2];
			person.learnedProfessions = new List<string>
			{
				ucProfessions[random.Next(0, ucProfessions.Length)]
			};
		}
		else if (c <= 30)
		{
			person._class = classes[1];
			person.learnedProfessions = new List<string>
			{
				mcProfessions[random.Next(0, mcProfessions.Length)]
			};
		}
		else if (c <= 100)
		{
			person._class = classes[0];
			var ran1 = random.Next(0, wcProfessions.Length);
			var ran2 = random.Next(0, wcProfessions.Length);

			while (ran1 == ran2) { ran2 = random.Next(0, wcProfessions.Length); }

			person.learnedProfessions = new List<string>
			{
				wcProfessions[ran1],
				wcProfessions[ran2]
			};
		}

		return person;
	}

	public static Person CreatePerson(int seed, string profession)
	{
		var person = CreatePerson(seed);

		if (!profession.Equals(string.Empty) && !person.learnedProfessions.Contains(profession))
		{
			person.learnedProfessions.Add(profession);
		}

		return person;
	}
}

