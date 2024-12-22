<template>
  <div class="voting-container">
    <!-- Timer (Only show when answering is not done) -->
    <div v-if="!isAnsweringDone" class="timer">
      <h2>Time left: {{ timeLeft }} seconds</h2>
    </div>

    <div v-if="!isAnsweringDone">
      <!-- Question and Answers -->
      <div class="question-section">
        <h2>
          Round {{ currentGroup?.roundId }} - Group
          {{ currentGroup?.groupNumber }}
        </h2>
        <p class="question">{{ currentGroup?.question?.questionText }}</p>
        <div><button @click="skipToNextGroup">Next</button></div>

        <div v-if="currentGroup?.answers?.length > 0" class="answers">
          <h3>Answers:</h3>
          <div
            v-for="answer in currentGroup.answers"
            :key="answer.id"
            class="answer-card"
            :class="{ voted: votedAnswerId === answer.id }"
          >
            <p>{{ answer.answerText }}</p>
            <button
              v-if="!isParticipant && !hasVoted"
              @click="voteForAnswer(answer.id)"
            >
              Vote
            </button>
            <span v-if="votedAnswerId === answer.id">
              You voted for this answer
            </span>
          </div>
        </div>
        <p v-else>No answers submitted for this group.</p>
      </div>

      <!-- Voting Progress -->
      <div class="progress">
        <button @click="skipToNextGroup" v-if="timeLeft === 0">Next</button>
      </div>
    </div>

    <!-- Player Table -->
    <div v-else>
      <h2>Leaderboard</h2>
      <table class="leaderboard-table">
        <thead>
          <tr>
            <th>Rank</th>
            <th>Player</th>
            <th>Points</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(player, index) in sortedPlayers" :key="player.id">
            <td>{{ index + 1 }}</td>
            <td>{{ player.username }}</td>
            <td>{{ playerPointsMap[player.id] || 0 }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Navigation (Only visible when answering is done) -->
    <div v-if="isAnsweringDone" class="navigation">
      <button @click="goToLobby">Go to Lobby</button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, onUnmounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import { useUserStore } from "@/stores/userStore";

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

// Game & Group Details
let roundIndex = ref<number | null>(null);
let groupIndex = ref<number | null>(null);

let currentGroup = ref<any>(null);
let gameDetails = ref<any>(null);

// Voting States
const hasVoted = ref<boolean>(false);
const votedAnswerId = ref<number | null>(null);
const isParticipant = ref<boolean>(false);

//Voting variables
let playerPointsMap = ref<Record<number, number>>({});

// Timer State
const timeLeft = ref<number>(0);
let timer: number | null = null;

const playerId = ref<number | null>(null);
const gameId = ref<number | null>(null);

const isFirstLoad = ref<boolean>(true);

const isAnsweringDone = ref<boolean>(false);


const startLeaderboardTimer = () => {
  stopTimer(); // Ensure no previous timer is running
  timeLeft.value = 60; // Start from 60 seconds
  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      goToLobby(); // Redirect to Lobby when timer finishes
    }
  }, 1000);
};

const sortedPlayers = computed(() => {
  // console.log(`107 ${JSON.stringify(gameDetails, null, 2)}`);
  console.log(`107 ${JSON.stringify(gameDetails.value?.players, null, 2)}`);
  return (gameDetails.value?.players || [])
    .slice() // Create a shallow copy to avoid mutating the original array
    .sort(
      (a: { points: number }, b: { points: number }) => b.points - a.points
    );
});

// Fetch Game ID
const fetchGameId = async () => {
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Game/Player/${playerId.value}`
    );
    gameId.value = response.data;
    localStorage.setItem("currentGameId", gameId.value.toString());
  } catch (err) {
    console.error("Failed to fetch game ID:", err);
  }
};

//Fetch Each Player`s Points
const fetchAllPlayerPoints = async (
  gId: number
): Promise<Record<number, number>> => {
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/PlayerPoint/find-by-gameId/${gId}`
    );

    const playerPoints = response.data as {
      playerId: number;
      gameId: number;
      points: number;
    }[];
    const pointsMap: Record<number, number> = {};

    // Aggregate points for each player
    playerPoints.forEach((pPoint) => {
      pointsMap[pPoint.playerId] =
        (pointsMap[pPoint.playerId] || 0) + pPoint.points;
    });

    console.log(`pointsMap: ${JSON.stringify(pointsMap,null,2)}`);

    return pointsMap;
  } catch (error) {
    console.error("Error fetching all player points:", error);
    throw new Error("Failed to fetch player points.");
  }
};

// Fetch Game Details
const fetchGameDetails = async () => {
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Game/${gameId.value}`
    );
    gameDetails.value = response.data;
    console.log(`game details: ${(response.data, null, 2)}`);
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// Submit Vote
const voteForAnswer = async (answerId: number) => {
  // Find the answer object based on the answerId
  const selectedAnswer = currentGroup.value.answers.find(
    (answer: { id: number }) => answer.id === answerId
  );

  if (!selectedAnswer || !selectedAnswer.playerId) {
    console.error("Selected answer or player ID not found.");
    return;
  }
  const request = {
    playerId: selectedAnswer.playerId,
    gameId: gameId.value,
    points: 100,
  };
  console.log(`PP request: ${JSON.stringify(request, null, 2)}`);
  try {
    await axios.post(`https://localhost:7269/Backend/PlayerPoint`, request);

    votedAnswerId.value = answerId;
    hasVoted.value = true;
  } catch (err) {
    console.error("Failed to submit vote:", err);
  }
};

// Timer Logic
const startTimer = () => {
  stopTimer();
  timeLeft.value = gameDetails.value.timerForVotingInSec || 999999; // Default 30 seconds
  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      skipToNextGroup();
    }
  }, 1000);
};

const stopTimer = () => {
  if (timer !== null) {
    clearInterval(timer);
    timer = null;
  }
};

const loadFirstGroup = async () => {
  if (!gameDetails.value || !gameDetails.value.gameRounds) {
    console.error("Game details or game rounds are not available.");
    return;
  }

  // Устанавливаем roundIndex и groupIndex на первую группу
  roundIndex.value = 1; // Первый раунд
  groupIndex.value = 1; // Первая группаF

  // Обновляем currentGroup, присваиваем данные для первой группы
  const firstRound = gameDetails.value.gameRounds[0];
  const firstGroup = firstRound.groups[0];

  currentGroup.value = firstGroup;
  startTimer(); // Стартуем таймер для первой группы
  isFirstLoad.value = false;
};

// Navigate to Next Group
const skipToNextGroup = async () => {
  if (!gameDetails.value || !gameDetails.value.gameRounds) {
    console.error("Game details or game rounds are not available.");
    return;
  }

  if (isFirstLoad.value) {
    // Set to the first group if it's the first load
    roundIndex.value = 1; // First round
    groupIndex.value = 1; // First group
    isFirstLoad.value = false; // Mark the first load as completed
  }

  // Existing logic for skipping to the next group...
  const currentRound = gameDetails.value.gameRounds.find(
    (round: { roundNumber: number }) => round.roundNumber === roundIndex.value
  );

  if (!currentRound) {
    console.error("Current round not found.");
    return;
  }

  const currentGroups = currentRound.groups || [];

  const currentGroupIndex = currentGroups.findIndex(
    (group: { groupNumber: number }) => group.groupNumber === groupIndex.value
  );

  if (currentGroupIndex < currentGroups.length - 1) {
    groupIndex.value = currentGroups[currentGroupIndex + 1].groupNumber;
  } else {
    const nextRoundIndex = gameDetails.value.gameRounds.findIndex(
      (round: { roundNumber: number }) => round.roundNumber === roundIndex.value
    );

    if (nextRoundIndex < gameDetails.value.gameRounds.length - 1) {
      roundIndex.value =
        gameDetails.value.gameRounds[nextRoundIndex + 1].roundNumber;
      groupIndex.value =
        gameDetails.value.gameRounds[nextRoundIndex + 1]?.groups[0]
          ?.groupNumber || 1;
    } else {
      await fetchGameDetails();
      console.log(`gameId: ${gameId.value}`);
      if (gameId.value) {
        playerPointsMap.value = await fetchAllPlayerPoints(gameId.value);
      }
      console.log("Voting complete! Redirect to summary.");
      isAnsweringDone.value = true;
      return;
    }
  }

  const newCurrentGroup = gameDetails.value.gameRounds
    .find(
      (round: { roundNumber: number }) => round.roundNumber === roundIndex.value
    )
    ?.groups.find(
      (group: { groupNumber: number }) => group.groupNumber === groupIndex.value
    );

  currentGroup.value = newCurrentGroup;
  hasVoted.value = false;
  startTimer();
};
const goToLobby = () => {
  router.push({ name: "Lobby" }); // Replace "Lobby" with the actual route name or path for your lobby page
};

// Lifecycle Hooks
onMounted(async () => {
  userStore.loadUser();
  if (!userStore.userId) {
    console.error("User not logged in.");
    return;
  }

  playerId.value = userStore.userId;

  await fetchGameId();
  if (!gameId.value) {
    console.error("Game ID not available.");
    return;
  }

  await fetchGameDetails();
  if (!gameDetails.value) {
    console.error("Game details not available.");
    return;
  }

  // Initialize to the first round and group if they are not set
  if (roundIndex.value === null || groupIndex.value === null) {
    roundIndex.value = 1; // Start from the first round
    groupIndex.value = 1; // Start from the first group
  }
  if (isFirstLoad) {
    await loadFirstGroup();
  } else {
    await skipToNextGroup(); // Load the first group
  }
  if (roundIndex.value !== null && groupIndex.value !== null) {
    startTimer();
  }
});

onUnmounted(() => {
  stopTimer();
});
</script>

<style scoped>
.leaderboard-table {
  width: 80%;
  margin: 20px auto;
  border-collapse: collapse;
  background-color: #ffffff;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.leaderboard-table thead {
  background-color: #7d3c98;
  color: white;
  text-align: left;
}

.leaderboard-table th,
.leaderboard-table td {
  padding: 15px 20px;
  text-align: center;
  font-size: 16px;
}

.leaderboard-table tbody tr:nth-child(odd) {
  background-color: #f7f3fc;
}

.leaderboard-table tbody tr:nth-child(even) {
  background-color: #e0d4f7;
}

.leaderboard-table tbody tr:hover {
  background-color: #d9c5e5;
  transform: scale(1.02);
  transition: all 0.2s ease-in-out;
}

.leaderboard-table th {
  font-size: 18px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.leaderboard-table td {
  font-size: 16px;
}

.leaderboard-table tbody tr {
  transition: all 0.3s;
}

.leaderboard-table td:first-child {
  font-weight: bold;
  color: #7d3c98;
}
.voting-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
  background-color: #f4f0ff;
  height: 100vh;
}

.timer {
  background-color: #c7a6d9;
  color: #fff;
  padding: 15px 30px;
  border-radius: 10px;
  font-size: 20px;
  font-weight: bold;
  margin-bottom: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.question-section {
  text-align: center;
  padding: 20px;
  background-color: #e0d4f7;
  border-radius: 15px;
  width: 80%;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.question-section h2 {
  font-size: 24px;
  font-weight: bold;
  color: #7d3c98;
  margin-bottom: 10px;
}

.question-section .question {
  background-color: #c7a6d9;
  color: #fff;
  padding: 10px;
  border-radius: 10px;
  margin: 20px 0;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.answers {
  margin-top: 20px;
  text-align: left;
}

.answer-card {
  background-color: #d9c5e5;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.answer-card.voted {
  background-color: #7d3c98;
  color: #fff;
}

button {
  background-color: #7d3c98;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 14px;
  cursor: pointer;
  transition: background-color 0.3s, box-shadow 0.3s;
}

button:hover {
  background-color: #5b2c6f;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.3);
}

.progress {
  margin-top: 20px;
}

.progress button {
  background-color: #5b2c6f;
  color: #fff;
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  border: none;
  transition: background-color 0.3s, box-shadow 0.3s;
}

.progress button:hover {
  background-color: #3e1d47;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.3);
}
</style>
