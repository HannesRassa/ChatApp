<template>
    <div class="question-list">
      <h2>Available Questions</h2>
      <ul>
        <li v-for="question in questions" :key="question.QuestionID">
          {{ question.QuestionText }}
        </li>
      </ul>
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
    </div>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        questions: [],
        errorMessage: '',
      };
    },
    created() {
      this.fetchQuestions();
    },
    methods: {
      async fetchQuestions() {
        try {
          const response = await axios.get('http://localhost:5180/Backend/Questions');
          this.questions = response.data;
        } catch (error) {
          this.errorMessage = error.response?.data || 'Failed to fetch questions.';
          console.error("Error fetching questions:", this.errorMessage);
        }
      }
    }
  };
  </script>
  
  <style scoped>
  .question-list {
    padding: 20px;
    max-width: 600px;
    margin: auto;
  }
  
  .error {
    color: red;
  }
  </style>
  