<template>
    <div class="pack-details">
      <!-- Navigation Buttons -->
      <div class="navigation-buttons">
        <button class="home-button" @click="goHome">Home</button>
        <button class="back-button" @click="goBack">Back</button>
      </div>
  
      <h1 class="page-title">Questions in "{{ packName }}"</h1>
      <ul v-if="questions.length > 0" class="question-list">
        <li v-for="question in questions" :key="question.id" class="question-item">
          {{ question.questionText }}
        </li>
      </ul>
      <p v-else class="no-questions">No questions available in this pack.</p>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent, ref, onMounted } from "vue";
  import axios from "axios";
  import { useRoute, useRouter } from "vue-router";
  
  export default defineComponent({
    name: "QuestionPackDetails",
    setup() {
      const route = useRoute();
      const router = useRouter();
      const packName = decodeURIComponent(route.params.packName as string);
      const questions = ref([]);
  
      const fetchQuestions = async () => {
        try {
          const response = await axios.get(`https://localhost:7269/Backend/Question/ByPack/${packName}`);
          questions.value = response.data;
        } catch (error) {
          console.error("Failed to fetch questions:", error);
        }
      };
  
      const goHome = () => {
        router.push("/");
      };
  
      const goBack = () => {
        router.push("/QuestionPacks");
      };
  
      onMounted(() => {
        fetchQuestions();
      });
  
      return {
        packName,
        questions,
        goHome,
        goBack,
      };
    },
  });
  </script>
  
  <style scoped>
  .pack-details {
    max-width: 600px;
    margin: 0 auto;
    padding: 20px;
    background-color: #f9f9f9;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }
  
  .page-title {
    font-size: 24px;
    text-align: center;
    margin-bottom: 20px;
    color: #333;
  }
  
  .question-list {
    list-style: none;
    padding: 0;
  }
  
  .question-item {
    margin: 10px 0;
    padding: 8px;
    background-color: #f3f3f3;
    border-radius: 4px;
  }
  
  .no-questions {
    color: #666;
    text-align: center;
    margin-top: 20px;
  }
  
  .navigation-buttons {
    display: flex;
    justify-content: space-between;
    margin-bottom: 20px;
  }
  
  .home-button,
  .back-button {
    background-color: #9104a3;
    color: white;
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
  }
  </style>
  