<template>
  <div class="voting-container">
    <!-- Timer -->
    <div class="timer">
      <h2>Voting Time: {{ timeLeft }} seconds</h2>
    </div>

    <!-- Question and Answers -->
    <div class="question-section">
      <h2>Round {{ roundIndex }} - Group {{ groupIndex }}</h2>
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
          <span v-if="votedAnswerId === answer.id"
            >You voted for this answer</span
          >
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
let roundIndex = ref<number | null>(1);
let groupIndex = ref<number | null>(1);

let currentGroup = ref<any>(null);
let gameDetails = ref<any>(null);

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
      `https://localhost:7269/Backend/Game/Player/${playerId.value}`
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
      `https://localhost:7269/Backend/Game/${gameId.value}`
    );
    gameDetails.value = response.data;
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// const findFirstRoundNGroupData = async () => {
//   if (!gameDetails.value || !gameDetails.value.gameRounds) {
//     console.error("gameDetails or gameRounds is not defined.");
//     return;
//   }

//   const firstRound = gameDetails.value.gameRounds[0];
//   if (!firstRound || !firstRound.groups || firstRound.groups.length === 0) {
//     console.error("No groups available in the first round.");
//     return;
//   }

//   roundIndex.value = firstRound.roundNumber;
//   groupIndex.value = firstRound.groups[0].groupNumber;
//   currentGroup.value = firstRound.groups[0];
// };

// Fetch Group Data
// const fetchGroupData = async () => {
//   try {
//     const response = await axios.get(
//       `https://localhost:7269/Backend/Group/${groupID.value}`
//     );
//     currentGroup.value = response.data;

//     // Check if the current player participated
//     isParticipant.value = currentGroup.value.players.some(
//       (player: any) => player.id === playerId.value
//     );

//     // Reset voting state
//     hasVoted.value = false;
//     votedAnswerId.value = null;
//   } catch (err) {
//     console.error("Failed to fetch group data:", err);
//   }
// };

// Submit Vote
const voteForAnswer = async (answerId: number) => {
  try {
    await axios.post(`https://localhost:7269/Backend/PlayerPoints`, {
      playerId: playerId.value,
      gameId: gameId.value,
      points: 100,
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

// Navigate to Next Group
const skipToNextGroup = async () => {
  if (!gameDetails.value || !gameDetails.value.gameRounds) {
    console.error("Game details or game rounds are not available.");
    return;
  }
  // Find the current round using roundNumber
  const currentRound = gameDetails.value.gameRounds.find(
    (round: { roundNumber: globalThis.Ref<number, number> }) =>
      round.roundNumber === roundIndex
  );
  // console.log(`179 STNG gamedata: ${JSON.stringify(gameDetails.value,null,2)}`);

  console.log(`179 STNG currendRound: ${JSON.stringify(currentRound,null,2)}`);
  console.log(`179 STNG roundIndex: ${roundIndex.value}`);

  // Extract groups from the current round
  const currentGroups = currentRound?.groups || [];

  // Find the index of the current group using groupNumber
  const currentGroupIndex = currentGroups.findIndex(
    (group: { groupNumber: globalThis.Ref<number, number> }) =>
      group.groupNumber === groupIndex
  );

  if (currentGroupIndex < currentGroups.length - 1) {
    // Move to the next group within the same round
    groupIndex = currentGroups[currentGroupIndex + 1].groupNumber;
  } else {
    // Move to the first group of the next round
    const nextRoundIndex = gameDetails.value.gameRounds.findIndex(
      (round: { roundNumber: globalThis.Ref<number, number> }) =>
        round.roundNumber === roundIndex
    );

    if (nextRoundIndex < gameDetails.value.gameRounds.length - 1) {
      // Increment roundIndex to move to the next round
      roundIndex = gameDetails.value.gameRounds[nextRoundIndex + 1].roundNumber;

      // Set groupIndex to the first group of the new round
      groupIndex =
        gameDetails.value.gameRounds[nextRoundIndex + 1]?.groups[0]
          ?.groupNumber || 1;
    } else {
      // No more rounds left
      console.log("Voting complete! Redirect to summary.");
      router.push("/Game/Summary");
      return;
    }
  }

  // fetchGroupData();
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
  if (!gameId.value) {
    console.error("Game ID not available.");
    return;
  }

  await fetchGameDetails();
  if (!gameDetails.value) {
    console.error("Game details not available.");
    return;
  }

  await skipToNextGroup();
  // await findFirstRoundNGroupData();

  if (roundIndex.value !== null && groupIndex.value !== null) {
    startTimer();
  }
});

onUnmounted(() => {
  stopTimer();
});
</script>

<style scoped>
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
