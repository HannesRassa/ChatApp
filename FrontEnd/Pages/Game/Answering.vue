<template>
  <div class="game-container">
    <!-- Players Section -->
    <div class="players-list">
      <h2>Players</h2>
      <ul>
        <li v-for="player in currentGroup?.players" :key="player.id">
          {{ player.username }}
        </li>
      </ul>
    </div>

    <!-- Main Section -->
    <div class="main-content">
      <div v-if="loading">Loading...</div>
      <div v-else-if="error">Error: {{ error }}</div>
      <div v-else>
        <h2 class="timer">{{ timeLeft }} seconds</h2>

        <div class="question">
          <p>{{ currentGroup?.question?.questionText }}</p>
        </div>

        <div v-if="!isAnswerSubmitted" class="submit-answer">
          <input
            v-model="answer"
            type="text"
            placeholder="Enter your answer"
            @keydown.enter="submitAnswer"
          />
          <button @click="submitAnswer">Submit Answer</button>
        </div>

        <div v-else class="submitted-answer">
          <h2>Submitted Answer</h2>
          <p>{{ submittedAnswer }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";
import { useUserStore } from "@/stores/userStore";

//UserStore variables
const userStore = useUserStore();
const router = useRouter();

//Player`s Id
const playerId = ref<number | null>(null);

//Game details
const gameId = ref<number | null>(null);
const gameDetails = ref<any>(null);

//Timer variables
const roundTime = ref<number>(10);
const timeLeft = ref<number>(roundTime.value);

//Variables for navigating between rounds
const currentGroup = ref<any>(null);
let roundID = ref<number | null>(null);
let groupID = ref<number | null>(null);
let currentRoundIndex: number = 1;
let currentGroupIndex: number = 1;

//variables for answering
const answer = ref<string>("");
const isAnswerSubmitted = ref<boolean>(false);
const submittedAnswer = ref<string>("");

//other variables
const loading = ref<boolean>(false);
const error = ref<string | null>(null);

let timer: number | null = null;

// Fetch game ID for the player
const fetchGameId = async () => {
  if (!playerId.value) return;
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Game/Player/${playerId.value}`
    );
    gameId.value = response.data;
    console.log(`fetched game id ${gameId.value}`);
  } catch (err) {
    console.error("Failed to fetch game ID:", err);
  }
};

//Find group with userId in Round
const findCurrentGroup = async () => {
  const userId = userStore.userId;
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Game/find-group/${gameId.value}/${currentRoundIndex}/${userId}`
    );
    groupID.value = response.data; // Ensure this sets a valid value
    console.log(`Group ID set to: ${groupID.value}`);
  } catch (error) {
    console.error("Error fetching the group:", error);
    throw error;
  }
};

// Fetch game details
const fetchGameDetails = async () => {
  if (!gameId.value) return;

  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Game/${gameId.value}`
    );
    gameDetails.value = response.data;

    // console.log(`fetched game details ${JSON.stringify(gameDetails.value,null,2)}}`);

    timeLeft.value = gameDetails.value?.timerForAnsweringInSec || 30; // Default to 30 seconds
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// Fetch group data
const fetchGroupData = async () => {
  console.log(
    `Before fetchGroupData: roundID=${roundID.value}, groupID=${groupID.value}`
  );

  try {
    loading.value = true;
    error.value = null;

    if (!groupID.value) throw new Error("Group ID is not defined.");
    const response = await axios.get(
      `https://localhost:7269/Backend/Group/${groupID.value}`
    );
    currentGroup.value = response.data;
    roundID = currentGroup.value.roundId;

    console.log(
      `after fetchGroupData: roundID=${roundID.value}, groupID=${groupID.value}`
    );

    console.log(
      `Fetched current group: ${JSON.stringify(currentGroup.value, null, 2)}`
    );
  } catch (err) {
    error.value = "Failed to fetch group data.";
    console.error(err);
  } finally {
    loading.value = false;
  }
};

// Go to the next round
const goToNextGroupOrRound = async () => {
  if (currentRoundIndex < gameDetails.value.rounds) {
    currentRoundIndex++;
    await findCurrentGroup();
    await fetchGroupData();
    stopTimer(); // Stop the current timer
    timeLeft.value = roundTime.value; // Reset the timer to the initial round time
    startTimer(); // Start the timer again for the new round
    isAnswerSubmitted.value = false;
  } else {
    // No more rounds left, end the game
    alert("Insert Logic to Redirect");
    router.push(`/game/voting`);
  }
};

// Submit Answer
const submitAnswer = async () => {
  console.log;
  if (!answer.value.trim()) {
    alert("Answer cannot be empty.");
    return;
  }
  const request = {
    groupId: groupID.value,
    playerId: playerId.value,
    question: currentGroup.value.question,
    roundId: roundID,
    answerText: answer.value,
  };
  console.log(`submit ans post request: ${JSON.stringify(request, null, 2)}`);
  try {
    await axios.post(`https://localhost:7269/Backend/Answer/`, request);
    submittedAnswer.value = answer.value;
    isAnswerSubmitted.value = true;
  } catch (err) {
    console.error("Failed to submit answer:", err);
  }
  answer.value = "";
};

// Timer logic
const startTimer = () => {
  stopTimer();
  timeLeft.value = gameDetails.value.timerForAnsweringInSec;

  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      goToNextGroupOrRound();
    }
  }, 1000);
};

const stopTimer = () => {
  if (timer !== null) {
    clearInterval(timer);
    timer = null;
  }
};

// Lifecycle hooks
onMounted(async () => {
  userStore.loadUser();

  if (!userStore.userId) {
    console.error("User ID is not set. Ensure the user is logged in.");
    return;
  }

  try {
    playerId.value = userStore.userId;

    await fetchGameId();
    await fetchGameDetails();
    await findCurrentGroup();
    await fetchGroupData();

    startTimer();
  } catch (err) {
    console.error("Error during initialization:", err);
  }
});

onUnmounted(() => stopTimer());
</script>

<style scoped>
.game-container {
  display: flex;
  height: 100vh;
  padding: 20px;
  background-color: #f4f0ff;
}

.players-list {
  flex: 1;
  padding: 20px;
  background-color: #e0d4f7;
  border-right: 3px solid #a569bd;
  display: flex;
  flex-direction: column;
  align-items: center;
  border-radius: 15px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.players-list h2 {
  font-size: 24px;
  font-weight: bold;
  color: #5b2c6f;
}

.players-list ul {
  list-style-type: none;
  padding: 0;
}

.players-list li {
  background-color: #c7a6d9;
  color: #fff;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.main-content {
  flex: 2;
  padding: 20px;
  background-color: #e0d4f7;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border-radius: 15px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.timer {
  font-size: 26px;
  font-weight: bold;
  color: #7d3c98;
  margin-bottom: 20px;
}

.question {
  margin-top: 20px;
  background-color: #c7a6d9;
  padding: 15px;
  border-radius: 8px;
  width: 100%;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.submit-answer,
.submitted-answer {
  margin-top: 20px;
  padding: 15px;
  border-radius: 8px;
  width: 100%;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.submit-answer input {
  padding: 12px;
  width: 100%;
  margin-bottom: 10px;
  border-radius: 8px;
  border: 1px solid #c7a6d9;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

button {
  background-color: #7d3c98;
  color: white;
  padding: 15px 50px;
  border-radius: 12px;
  font-size: 16px;
  cursor: pointer;
  border: none;
  transition: background-color 0.3s, box-shadow 0.3s;
}

button:hover {
  background-color: #5b2c6f;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.3);
}

.submitted-answer h2 {
  font-size: 22px;
  font-weight: bold;
  color: #7d3c98;
}
</style>
