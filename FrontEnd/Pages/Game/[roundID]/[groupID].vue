<template>
  <div>
    <h1>Game Round {{ currentRoundIndex }} - Group {{ currentGroupIndex }}</h1>
    <div v-if="loading">Loading...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else>
      <div>
        <h2>Players</h2>
        <ul>
          <li v-for="player in currentGroup?.players" :key="player.id">
            {{ player.username }}
          </li>
        </ul>
      </div>
      <div>
        <h2>Question</h2>
        <p>{{ currentGroup?.question?.questionText }}</p>
      </div>
      <div>
        <h2>Your Answer</h2>
        <input v-model="answer" type="text" placeholder="Enter your answer" />
        <button @click="submitAnswer">Submit Answer</button>
      </div>
      <div>
        <h2>Time Left: {{ timeLeft }} seconds</h2>
      </div>
    </div>
    <button @click="goToNextGroupOrRound">Next Round</button>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore();
const route = useRoute();

const playerId = userStore.userId;
const roundID = ref<number>(Number(route.params.roundID));
const groupID = ref<number>(Number(route.params.groupID));
const questionId = ref<number>(0);

let currentRoundIndex: number = 1;
let currentGroupIndex: number = 1;

const gameData = ref<any>(null);
const currentGroup = ref<any>(null);

const loading = ref(true);
const roundTime = ref<number>(420)
const error = ref<string | null>(null);
const gameId = ref<number | null>(null);
const answer = ref<string>("");

const timeLeft = ref<number>(roundTime.value);
let timer: number | null = null;


const fetchLoggedInUserGameId = async (): Promise<number | null> => {
  try {
    const userId = userStore.userId;
    console.log(playerId);
    console.log(userId);
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/Player/${userId}`,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    const gameId = response.data;
    console.log(gameId);
    return gameId;
  } catch (error) {
    console.error("Error fetching game room ID:", error);
    return null;
  }
};

const fetchGroupData = async () => {
  try {
    loading.value = true;
    const response = await axios.get(
      `http://localhost:5180/Backend/Group/${groupID.value}`
    );
    currentGroup.value = response.data;
  } catch (err) {
    console.error(err);
    error.value = "Failed to fetch group data.";
  } finally {
    loading.value = false;
  }
};

const findCurrentGroup = async () => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/find-group/${currentRoundIndex}/1` //${playerId.value}
    );  

    return response.data; // Return the data from the response
  } catch (error) {
    console.log(`error for findCurentGroup Resposne:${currentRoundIndex},${playerId}`);

    console.error("Error fetching the group:", error);
    throw error; // Re-throw the error if needed
  }
};
const fetchGameData = async () => {
  try {
    loading.value = true;
    const response = await axios.get(`http://localhost:5180/Backend/Game/${gameId.value}`); //${gameId.value}
    gameData.value = response.data;
    roundTime.value = gameData.value.timerForAnsweringInSec;
    console.log(`gameData: ${JSON.stringify(gameData.value, null, 2)}`);
  } catch (err) {
    console.error(err);
    error.value = "Failed to fetch game data.";
  } finally {
    loading.value = false;
  }
};

const submitAnswer = async () => {
  if (!answer.value.trim()) {
    alert("Answer cannot be empty.");
    return;
  }
  try {
    await axios.post(`http://localhost:5180/Backend/Answer/`, {
      groupId: groupID.value,
      playerId,
      questionId: currentGroup.value.question.id,
      roundId: roundID.value,
      questionText: currentGroup.value.question.questionText.value,
      answerText: answer.value,
    });
    alert("Answer submitted successfully!");
    answer.value = "";
  } catch (err) {
    console.error(err);
    console.log(
      `grId:${groupID}, plId:${playerId},ansId:${questionId} rndId:${roundID}, ansTxt:${
        answer.value
      }, questin:${JSON.stringify(currentGroup.value?.question)}`
    );
    alert("Failed to submit answer.");
  }
};

const goToNextGroupOrRound = async () => {
  if (currentRoundIndex < gameData.value.rounds) {
    currentRoundIndex++;
    stopTimer(); // Stop the current timer
    timeLeft.value = roundTime.value; // Reset the timer to the initial round time
    startTimer(); // Start the timer again for the new round
  } else {
    // No more rounds left, end the game
    alert("Insert Logic to Redirect");
  }
};
const startTimer = () => {
  stopTimer();
  timeLeft.value = roundTime.value;

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

onMounted(async () => {
  userStore.loadUser();
  const gameId = fetchLoggedInUserGameId();
  if (userStore.userId) {
    await fetchLoggedInUserGameId();
  }
  await fetchGroupData();
  await findCurrentGroup();
  startTimer();
});

onUnmounted(() => {
  stopTimer();
});
</script>

<style scoped>
h1 {
  text-align: center;
}
ul {
  list-style: none;
  padding: 0;
}
ul li {
  margin: 5px 0;
}
</style>
