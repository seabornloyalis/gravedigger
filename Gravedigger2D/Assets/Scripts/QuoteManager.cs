using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class QuoteManager : MonoBehaviour {

	public static QuoteManager instance = null;
	string[] ingameQuotes = {
		"Barry'd alive",
		"We will Barry you -Khrushchev",
		"We will not bury you with shovels. Your working class will Barry you -Khrushchev",
		"Barry the hatchet",
		"Catchphrase!",
		"Every man should keep a fair-sized cemetery in which to bury the faults of his friends. -Henry Ward Beecher",
		"It is infinitely better to transplant a heart than to bury it to be devoured by worms -Christiaan Barnard",
		"Since you would save none of me, I bury some of you- John Donne",
		"My girls, you mess with them? I will bury you underground -Mark Wahlberg",
		"Virtually every civilized society today holds sacred the right to peaceably bury their dead- Mike Schmidt",
		"To keep oneself safe does not mean to bury oneself -Lucius Annaeus Seneca",
		"Don't bury me with anyone old- Tiny Tim"
	};
	string[] deathQuotes = {
		"That. Try not doing that.",
		"Who buries the gravedigger?",
		"At least there won't be much to bury.",
		"Ashes to ashes, dust to dust.",
		"Barry always liked this episode of Magic School Bus.",
		"Do you believe in reincarnation?",
		"That'll leave a mark. Or several.",
		"Something witty.",
		"At least you have a promising future as soylent green.",
		"Barry always wondered what someone inside out looked like.",
		"At the last moment, Barry regretted quitting smoking.",
		"At least the hedgemaze will be lush.",
		"Barry stopped digging graves and started serving dinner.",
		"Apparently Barry was too old for this s#%@.",
		"Swing low, sweet chariot!"
	};

	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		
		DontDestroyOnLoad (gameObject);
	}

	public string randLevelQuote () {
		int rand = Random.Range (0, ingameQuotes.Length);
		return ingameQuotes [rand];
	}

	public string randDeathQuote () {
		int rand = Random.Range (0, deathQuotes.Length);
		return deathQuotes [rand];
	}
}
