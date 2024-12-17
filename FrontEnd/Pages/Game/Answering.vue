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
        <h2 class="timer">{{ timeLeft }} seconds</h2>
  
        <div class="question">
          <p>{{ currentGroup?.question?.questionText }}</p>
        </div>
  
        <div v-if="!isAnswerSubmitted" class="submit-answer">
          <input v-model="answer" type="text" placeholder="Enter your answer" />
          <button @click="submitAnswer">Submit Answer</button>
        </div>
  
        <div v-else class="submitted-answer">
          <h2>Submitted Answer</h2>
          <p>{{ submittedAnswer }}</p>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, onMounted, onUnmounted } from "vue";
  import { useRouter } from "vue-router";
  import axios from "axios";
  import { useUserStore } from "@/stores/userStore";
  
  const userStore = useUserStore();
  const router = useRouter();
  
  const playerId = ref<number | null>(null);
  const gameId = ref<number | null>(null);
  const currentGroup = ref<any>(null);
  const timeLeft = ref<number>(0);
  const gameDetails = ref<any>(null);
  const roundID = ref<number | null>(null);
  const groupID = ref<number | null>(null);
  
  const answer = ref<string>("");
  const isAnswerSubmitted = ref<boolean>(false);
  const submittedAnswer = ref<string>("");
  
  let timer: number | null = null;
  
  // Fetch the current group and round dynamically for the player
  const fetchCurrentGroupAndRound = async () => {
    try {
      const response = await axios.get(
        `http://localhost:5180/Backend/Game/find-group/${gameId.value}/${playerId.value}`
      );
      const { roundId, groupId } = response.data; 
  
      roundID.value = roundId;
      groupID.value = groupId;
  
      await fetchGroupData();
    } catch (err) {
      console.error("Failed to fetch current group/round:", err);
    }
  };
  
  // Fetch the game ID for the player
  const fetchGameId = async () => {
    if (!playerId.value) return;

    try {
        const response = await axios.get(
        `http://localhost:5180/Backend/Game/Player/${playerId.value}`
        );
        gameId.value = response.data;
    } catch (err) {
        console.error("Failed to fetch game ID:", err);
    }
    };

  
  // Fetch game details
  const fetchGameDetails = async () => {
    if (!gameId.value) return;

    try {
        const response = await axios.get(
        `http://localhost:5180/Backend/Game/${gameId.value}`
        );
        gameDetails.value = response.data;

        timeLeft.value = gameDetails.value?.timerForAnsweringInSec || 30; // Default to 30 seconds
    } catch (err) {
        console.error("Failed to fetch game details:", err);
    }
    };

  
  // Fetch group data
  const fetchGroupData = async () => {
    if (!groupID.value) return;

    try {
        const response = await axios.get(
        `http://localhost:5180/Backend/Group/${groupID.value}`
        );
        currentGroup.value = response.data;
    } catch (err) {
        console.error("Failed to fetch group data:", err);
    }
    };

  
  // Submit Answer
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
        redirectToVoting();
      }
    }, 1000);
  };
  
  const stopTimer = () => {
    if (timer !== null) {
      clearInterval(timer);
      timer = null;
    }
  };
  
  // Redirect to voting when time is up
  const redirectToVoting = () => {
    router.push({ path: "/Game/Voting" });
  };
  
  // Lifecycle hooks
  onMounted(async () => {
    userStore.loadUser();
    
    // Check for logged-in user
    if (!userStore.userId) {
        console.error("User ID is not set. Ensure the user is logged in.");
        return;
    }

    try {
        playerId.value = userStore.userId;

        // Fetch gameId, gameDetails, and group data sequentially
        await fetchGameId(); // Fetch gameId first
        if (!gameId.value) throw new Error("Game ID could not be retrieved.");

        await fetchGameDetails(); // Fetch game details after gameId is available
        if (!gameDetails.value) throw new Error("Game details could not be retrieved.");

        await fetchCurrentGroupAndRound(); // Fetch group and round data
        if (!currentGroup.value) throw new Error("Current group data could not be retrieved.");

        timeLeft.value = gameDetails.value.timerForAnsweringInSec || 30; // Ensure timeLeft is set

        startTimer(); // Start timer after all data is initialized
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