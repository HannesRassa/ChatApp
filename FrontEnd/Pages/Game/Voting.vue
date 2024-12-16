<template>
    <div class="voting-container">
      <!-- Timer -->
      <div class="timer">
        <h2>Voting Time: {{ timeLeft }} seconds</h2>
      </div>
  
      <!-- Question and Answers -->
      <div class="question-section">
        <h2>Round {{ roundID }} - Group {{ groupID }}</h2>
        <p class="question">{{ currentGroup?.question?.questionText }}</p>
  
        <div v-if="currentGroup?.answers?.length > 0" class="answers">
          <h3>Answers:</h3>
          <div
            v-for="answer in currentGroup.answers"
            :key="answer.id"
            class="answer-card"
            :class="{ 'voted': votedAnswerId === answer.id }"
          >
            <p>{{ answer.answerText }}</p>
            <button
              v-if="!isParticipant && !hasVoted"
              @click="voteForAnswer(answer.id)"
            >
              Vote
            </button>
            <span v-if="votedAnswerId === answer.id">You voted for this answer</span>
          </div>
        </div>
        <p v-else>No answers submitted for this group.</p>
      </div>
  
      <!-- Voting Progress -->
      <div class="progress">
        <button @click="skipToNextGroup" v-if="timeLeft === 0">Next</button>
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
const roundID = ref<number>(1); // Start with Round 1
const groupID = ref<number>(1); // Start with Group 1
const currentGroup = ref<any>(null);
const gameDetails = ref<any>(null);

// Voting States
const hasVoted = ref<boolean>(false);
const votedAnswerId = ref<number | null>(null);
const isParticipant = ref<boolean>(false);

// Timer State
const timeLeft = ref<number>(0);
let timer: number | null = null;

const playerId = ref<number | null>(null);
const gameId = ref<number | null>(null);

// Fetch Game ID
const fetchGameId = async () => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/Player/${playerId.value}`
    );
    gameId.value = response.data;
    localStorage.setItem("currentGameId", gameId.value.toString());
  } catch (err) {
    console.error("Failed to fetch game ID:", err);
  }
};

// Fetch Game Details
const fetchGameDetails = async () => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/${gameId.value}`
    );
    gameDetails.value = response.data;
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// Fetch Group Data
const fetchGroupData = async () => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Group/${groupID.value}`
    );
    currentGroup.value = response.data;

    // Check if the current player participated
    isParticipant.value = currentGroup.value.players.some(
      (player: any) => player.id === playerId.value
    );

    // Reset voting state
    hasVoted.value = false;
    votedAnswerId.value = null;
  } catch (err) {
    console.error("Failed to fetch group data:", err);
  }
};

// Submit Vote
const voteForAnswer = async (answerId: number) => {
  try {
    await axios.post(`http://localhost:5180/Backend/Vote`, {
      playerId: playerId.value,
      answerId,
      groupId: groupID.value,
      roundId: roundID.value,
    });

    votedAnswerId.value = answerId;
    hasVoted.value = true;
  } catch (err) {
    console.error("Failed to submit vote:", err);
  }
};

// Timer Logic
const startTimer = () => {
  stopTimer();
  timeLeft.value = gameDetails.value.timerForVotingInSec || 30; // Default 30 seconds
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

// Navigate to Next Group
const skipToNextGroup = () => {
  const currentRound = gameDetails.value?.gameRounds.find(
    (round: any) => round.id === roundID.value
  );

  const currentGroups = currentRound?.groups || [];
  const currentGroupIndex = currentGroups.findIndex(
    (group: any) => group.id === groupID.value
  );

  if (currentGroupIndex < currentGroups.length - 1) {
    // Move to next group in the same round
    groupID.value = currentGroups[currentGroupIndex + 1].id;
  } else {
    // Move to first group of the next round
    const nextRoundIndex = gameDetails.value?.gameRounds.findIndex(
      (round: any) => round.id === roundID.value
    );

    if (nextRoundIndex < gameDetails.value.gameRounds.length - 1) {
      roundID.value = gameDetails.value.gameRounds[nextRoundIndex + 1].id;
      groupID.value =
        gameDetails.value.gameRounds[nextRoundIndex + 1]?.groups[0]?.id;
    } else {
      console.log("Voting complete! Redirect to summary.");
      router.push("/Game/Summary");
      return;
    }
  }

  fetchGroupData();
  startTimer();
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
  if (!gameId.value) return;

  await fetchGameDetails();
  await fetchGroupData();
  startTimer();
});

onUnmounted(() => {
  stopTimer();
});
</script>

  
  
  <style scoped>
  .answer {
    margin-bottom: 10px;
  }
  button {
    margin-left: 10px;
  }
  </style>
  