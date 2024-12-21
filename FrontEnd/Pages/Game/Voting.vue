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
import Answering from "./Answering.vue";

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

// Game & Group Details
let roundID = ref<number>(133722869); // Start with Round 1
const groupID = ref<number>(133722869); // Start with Group 1
const currentGroup = ref<any>(null);
const gameDetails = ref<any>(null);
const Questions = ref<any[]>([]);
const answers = ref<any[]>([]);

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

// Fetch Game Detailss
const fetchGameDetails = async () => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/${gameId.value}`
    );

    return response.data;
  } catch (err) {
    console.error("Failed to fetch game details:", err);
  }
};

// Fetch all questions from the game
const findAllGameQuestions = () => {
  Questions.value = [];
  gameDetails.value.gameRounds.forEach((round: any) => {
    round.groups.forEach((group: any) => {
      Questions.value.push(group.question);
    });
  });
  console.log(JSON.stringify(Questions.value, null, 2));
};

//Find All Answers and save them to a variable
const extractAnswersFromGameDetails = (): void => {
  console.log(
      "Fetched gameDetails:",
      JSON.stringify(gameDetails.value, null, 2)
    );
  if (!gameDetails || !gameDetails.value?.gameRounds) {
    console.error("gameDetails or gameRounds is undefined:", gameDetails);
    return;
  }
  if (!gameDetails) {
    console.log(`game Details incorrect${gameDetails}`);
  }

  answers.value = []; // Clear the previous answers
  gameDetails.value?.gameRounds.forEach((round: any) => {
    console.log(`Round ${round.roundNumber}:`);
    round.groups.forEach((group: any) => {
      console.log(`Group ${group.groupNumber}:`);
      group.answers.forEach((answer: any) => {
        console.log(`Answer ID ${answer.id}: ${answer.answerText}`);
        answers.value.push(answer); // Save each answer to the answers array
      });
    });
  });
  console.log(`Found Answers ${JSON.stringify(answers.value, null, 2)}`);
};

const findFirstRoundAndGroupId()=>{
  roundID = gameDetails.value.gameRounds[0].id;
  groupID = gameDetails.value.gameRounds[0].id;
};

// Submit Vote
const voteForAnswer = async (answerId: number) => {
  try {
    await axios.post(`http://localhost:5180/Backend/PlayerPoints`, {
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
  timeLeft.value = gameDetails.value.timerForVotingInSec || 9999; // Default 10 seconds
  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      // skipToNextGroup();
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

  gameDetails.value = await fetchGameDetails();
  findAllGameQuestions();
  extractAnswersFromGameDetails();
  startTimer();
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
