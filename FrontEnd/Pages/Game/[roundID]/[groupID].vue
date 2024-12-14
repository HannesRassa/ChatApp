<template>
  <div>
    <h1>Game Round {{ roundID }} - Group {{ groupID }}</h1>
    <div v-if="loading">Loading...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else>
      <div>
        <h2>Players</h2>
        <ul>
          <li v-for="player in group.players" :key="player.id">
            {{ player.username }}
          </li>
        </ul>
      </div>
      <div>
        <h2>Question</h2>
        <p>{{ group.question?.questionText }}</p>
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
    <button @click="goToNextRound">Next Round</button>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";

const route = useRoute();

const playerId = Number(1); //change to dynamic
const roundTime = 60; // Change to dynamic

const roundID = ref<number>(Number(route.params.roundID));
const groupID = ref<number>(Number(route.params.groupID));
const group = ref<any>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const answer = ref<string>("");
const timeLeft = ref<number>(roundTime);
let timer: number | null = null;

const fetchGroupData = async () => {
  try {
    loading.value = true;
    const response = await axios.get(
      `http://localhost:5180/Backend/Group/${groupID.value}`
    );
    group.value = response.data;
  } catch (err) {
    console.error(err);
    error.value = "Failed to fetch group data.";
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
      groupId:groupID.value,
      playerId,
      roundId:roundID.value,
      questionText: group.value.question.questionText.value,
      answerText: answer.value,
    });
    alert("Answer submitted successfully!");
    answer.value = "";
  } catch (err) {
    console.error(err);
    console.log(
      `grId:${groupID}, plId:${playerId}, rndId:${roundID}, ansTxt:${
        answer.value
      }, questin:${JSON.stringify(group.value?.question)}`
    );
    alert("Failed to submit answer.");
  }
};

const goToNextRound = async () => {
  alert("Time is up! Moving to the next round.");

  try {
    // Making an API call to move to the next round
    const response = await axios.post(
      `http://localhost:5180/Backend/Game/${roundID}/next-round`
    );
    console.log(`NextRound post response${response}`);

    if (response.status === 200) {
      // Update the roundID and groupID based on the new round data
      roundID.value = response.data.roundID;
      groupID.value = response.data.groupID; 

      // Refetch the group data for the new round
      fetchGroupData();
    } else {
      alert("Failed to transition to the next round.");
    }
  } catch (error) {
    console.error(error);
    alert("Error transitioning to the next round.");
  }
};
const startTimer = () => {
  timer = window.setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      clearInterval(timer!);
      timer = null;
      goToNextRound();
    }
  }, 1000);
};

const stopTimer = () => {
  if (timer !== null) {
    clearInterval(timer);
    timer = null;
  }
};

onMounted(() => {
  fetchGroupData();
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
