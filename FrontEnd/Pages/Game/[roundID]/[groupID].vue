<template>
  <div class="game-container">
    <!-- Players Section on the Left -->
    <div class="players-list">
      <h2>Players</h2>
      <ul>
        <li v-for="player in currentGroup?.players" :key="player.id">
          {{ player.username }}
        </li>
      </ul>
    </div>

    <!-- Main Content Section -->
    <div class="main-content">
      <!-- Timer at the Top -->
      <h2 class="timer">{{ timeLeft }} seconds</h2>

      <!-- Question -->
      <div class="question">
        <p>{{ currentGroup?.question?.questionText }}</p>
      </div>

      <!-- Submit Answer Section -->
      <div v-if="!isAnswerSubmitted" class="submit-answer">
        <input v-model="answer" type="text" placeholder="Enter your answer" />
        <button @click="submitAnswer">Submit Answer</button>
      </div>

      <!-- Display Submitted Answer -->
      <div v-else class="submitted-answer">
        <h2>Submitted Answer</h2>
        <p>{{ submittedAnswer }}</p>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, onUnmounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore();
const route = useRoute();
const router = useRouter();

// Fetch and assign userId
onMounted(() => {
  userStore.loadUser(); // Load user data from localStorage
});

const playerId = ref<number | null>(null);
const gameId = ref<number | null>(null);

const roundID = ref<number>(Number(route.params.roundID));
const groupID = ref<number>(Number(route.params.groupID));
const currentGroup = ref<any>(null);
const timeLeft = ref<number>(0);
const gameDetails = ref<any>(null);

let timer: number | null = null;

const answer = ref<string>("");
const isAnswerSubmitted = ref<boolean>(false);
const submittedAnswer = ref<string>("");

// Fetch the gameId using the player's ID
const fetchGameId = async () => {
  try {
    const response = await axios.get(`http://localhost:5180/Backend/Game/Player/${playerId.value}`);
    gameId.value = response.data; // Assuming the response is a single gameId value
    localStorage.setItem("currentGameId", gameId.value.toString());
  } catch (err) {
    console.error("Failed to fetch game ID:", err);
  }
};


// Fetch game details
const fetchGameDetails = async () => {
  try {
    const response = await axios.get(`http://localhost:5180/Backend/Game/${gameId.value}`);
    gameDetails.value = response.data;

    // Set the timer duration from the game entity
    timeLeft.value = gameDetails.value.timerForAnsweringInSec;
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// Fetch current group data
const fetchGroupData = async () => {
  try {
    const response = await axios.get(`http://localhost:5180/Backend/Group/${groupID.value}`);
    currentGroup.value = response.data;
  } catch (err) {
    console.error("Failed to fetch group data:", err);
  }
};

// Submit answer logic
const submitAnswer = async () => {
  if (!answer.value.trim()) {
    alert("Answer cannot be empty.");
    return;
  }

  try {
    await axios.post(`http://localhost:5180/Backend/Answer/`, {
      groupId: groupID.value,
      playerId: playerId.value,
      questionId: currentGroup.value.question.id,
      roundId: roundID.value,
      answerText: answer.value,
    });
    submittedAnswer.value = answer.value;
    isAnswerSubmitted.value = true;
  } catch (err) {
    console.error("Failed to submit answer:", err);
  }
};

// Timer logic
const startTimer = () => {
  stopTimer();
  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      checkIfGameIsOver();
    }
  }, 1000);
};

const stopTimer = () => {
  if (timer !== null) {
    clearInterval(timer);
    timer = null;
  }
};

// Check if the game is over and redirect
const checkIfGameIsOver = () => {
  const isLastRound = roundID.value === gameDetails.value.gameRounds.at(-1).id;
  const isLastGroupInRound =
    groupID.value === gameDetails.value.gameRounds.find((round: any) => round.id === roundID.value)?.groups.at(-1).id;

  if (isLastRound && isLastGroupInRound) {
    // Navigate to the first group's Voting page of the first round
    const firstRoundID = gameDetails.value.gameRounds[0]?.id;
    const firstGroupID = gameDetails.value.gameRounds[0]?.groups[0]?.id;

    router.push({
      path: `/Game/${firstRoundID}/${firstGroupID}/Voting`,
    });
  } else {
    redirectToNextGroupOrRound();
  }
};

// Redirect to the next group or round
const redirectToNextGroupOrRound = () => {
  const currentRound = gameDetails.value.gameRounds.find(
    (round: any) => round.id === roundID.value
  );
  const currentGroups = currentRound?.groups || [];

  const currentGroupIndex = currentGroups.findIndex(
    (group: any) => group.id === groupID.value
  );

  if (currentGroupIndex < currentGroups.length - 1) {
    // Move to the next group within the same round
    router.push({
      path: `/Game/${roundID.value}/${currentGroups[currentGroupIndex + 1].id}`,
    });
  } else {
    // Move to the first group of the next round
    const nextRoundIndex = gameDetails.value.gameRounds.findIndex(
      (round: any) => round.id === roundID.value
    );

    if (nextRoundIndex < gameDetails.value.gameRounds.length - 1) {
      const nextRound = gameDetails.value.gameRounds[nextRoundIndex + 1];
      router.push({
        path: `/Game/${nextRound.id}/${nextRound.groups[0]?.id}`,
      });
    }
  }
};

// Lifecycle hooks
onMounted(async () => {
  userStore.loadUser();
  if (!userStore.userId) {
    console.error("User ID is not set. Ensure the user is logged in.");
    return;
  }
  playerId.value = userStore.userId;

  await fetchGameId();
  if (!gameId.value) {
    console.error("Game ID could not be fetched.");
    return;
  }

  await fetchGameDetails();
  await fetchGroupData();
  startTimer();
});

onUnmounted(() => {
  stopTimer();
});
</script>


<style scoped>
.game-container {
  display: flex;
  flex-direction: row;
  height: 100vh;
  padding: 20px;
  box-sizing: border-box;
}

.players-list {
  width: 20%;
  padding: 10px;
  border-right: 1px solid #ddd;
}

.players-list h2 {
  text-align: center;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.timer {
  font-size: 2rem;
  margin-bottom: 20px;
}

.question {
  font-size: 2rem;
  text-align: center;
  margin-bottom: 20px;
}

.submit-answer,
.submitted-answer {
  font-size: 2rem;
  text-align: center;
  margin-bottom: 20px;
}

input {
  padding: 5px;
  font-size: 1rem;
}

button {
  margin-top: 10px;
  padding: 10px 20px;
  font-size: 2rem;
  cursor: pointer;
}

.next-button {
  margin-top: 30px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 5px;
}
</style>