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
      </div>
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import axios from 'axios';
  
  const route = useRoute();
  const roundID = Number(route.params.roundID);
  const groupID = Number(route.params.groupID);
  
  const group = ref<any>(null);
  const loading = ref(true);
  const error = ref<string | null>(null);
  const answer = ref<string>('');
  
  // Fetch group data based on roundID and groupID
  const fetchGroupData = async () => {
    try {
      loading.value = true;
      const response = await axios.get(
        `http://localhost:5180/Backend/Group/${groupID}`
      );
      group.value = response.data;
    } catch (err) {
      console.error(err);
      error.value = 'Failed to fetch group data.';
    } finally {
      loading.value = false;
    }
  };
  
  // Submit the player's answer
  const submitAnswer = async () => {
    if (!answer.value.trim()) {
      alert('Answer cannot be empty.');
      return;
    }
  
    try {
      await axios.post(`http://localhost:5180/Backend/Game/submit`, {
        roundID,
        groupID,
        answer: answer.value,
      });
      alert('Answer submitted successfully!');
      answer.value = '';
    } catch (err) {
      console.error(err);
      alert('Failed to submit answer.');
    }
  };
  
  // Fetch group data on mount
  onMounted(fetchGroupData);
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
  